using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using HalfLight.Input;

namespace HalfLight.Interact
{
    public class InteractHandler : MonoBehaviour
    {
        #region Public Field
        public enum interactType
        {
            Fixed,
            Spring,
            ParentConstraint
        }

        public enum hand
        {
            leftHand,
            rightHand
        }
        #endregion

        #region Serialized Field
        [Header("Hand")]
        [SerializeField] private hand selectedHand;

        [Header("Grabbing")]
        [SerializeField] private interactType selectedGrab;
        [SerializeField] private bool gripHold;

        [SerializeField] private GameObject playerBody;
        #endregion

        #region Private Field
        private ControllerInput _inputs;
        public ControllerInput Inputs { get { return _inputs; } set { _inputs = value; } }
        private ControllerInput.InputValues _controller;
        public ControllerInput.InputValues Controller { get { return _controller;} set { _controller = value;} }
        private Rigidbody _rb;
        public Rigidbody RB { get { return _rb; } set { _rb = value; } }
        private string _handName;
        public string HandName { get { return _handName; } set { _handName = value; } }
        private ConstraintSource _constraintSource;
        public ConstraintSource ConstraintSource { get { return _constraintSource; } set { _constraintSource = value; } }

        public Vector3 prePos;
        public Vector3 dis;
        #endregion

        #region BuiltIn Methods
        private void Start()
        {
            #region Initializing Components
            Inputs = ControllerInput.Instance;
            RB = GetComponent<Rigidbody>();
            #endregion
            
            #region Select Hand
            GetHand(selectedHand);
            #endregion
        }

        private void Update()
        {
            isGrip(Controller);
        }
        #endregion

        #region Selected Hand Input
        private void GetHand(hand selectedHand)
        {
            switch (selectedHand)
            {
                case hand.leftHand:
                    Controller = Inputs.getLeftHand;
                    HandName = Enum.GetName(typeof(hand), 1);
                    break;

                case hand.rightHand:
                    Controller = Inputs.getRightHand;
                    HandName = Enum.GetName(typeof(hand), 2);
                    break;
            }
        }
        #endregion

        #region Check Grip
        private void isGrip(ControllerInput.InputValues Controller)
        {
            if (Controller.gripButtonPressed)
            {
                gripHold = true;
            }
            else
            {
                gripHold = false;
            }
        }
        #endregion

        private bool CanCreateJoint(GameObject collider)
        {
            if (collider.GetComponent<Joint>().connectedBody.name != HandName)
            {
                return true;
            }
            return false;
        }

        #region Grabbing & Climbing
        private void OnTriggerStay(Collider other)
        {
            GameObject collider = other.gameObject;

            #region Grabbing
            if (collider.GetComponent<Rigidbody>() != null
                    && collider.layer == LayerMask.NameToLayer("Grabable")
                        && collider.GetComponent<Joint>().connectedBody.name != HandName)
            {
                if (gripHold == true)
                {
                    #region Create Joint
                    switch (selectedGrab)
                    {
                        case interactType.Fixed:
                            if (collider.GetComponent<FixedJoint>() == null)
                            {
                                collider.AddComponent<FixedJoint>();
                                collider.GetComponent<FixedJoint>().connectedBody = RB;
                            }
                            break;

                        case interactType.Spring:
                            if (collider.GetComponent<SpringJoint>() == null)
                            {
                                collider.AddComponent<SpringJoint>();
                                collider.GetComponent<SpringJoint>().connectedBody = RB;
                            }
                            break;

                        case interactType.ParentConstraint:
                            #region Parent Constraint
                            // var parentConstraint = collider.gameObject.AddComponent<ParentConstraint>();
                            // constraintSource.sourceTransform = RB.transform;
                            // constraintSource.weight = 1f;

                            // parentConstraint.AddSource(constraintSource);
                            // parentConstraint.weight = 1f;
                            // parentConstraint.constraintActive = true;
                            // parentConstraint.locked = true;
                            break;
                            #endregion
                    }
                    #endregion

                    #region Loose Angular
                    if (_inputs.getRightHand.secondaryButtonPressed == true)
                    {
                        _rb.GetComponent<ConfigurableJoint>().angularXMotion = ConfigurableJointMotion.Free;
                        _rb.GetComponent<ConfigurableJoint>().angularYMotion = ConfigurableJointMotion.Free;
                        _rb.GetComponent<ConfigurableJoint>().angularZMotion = ConfigurableJointMotion.Free;
                    }
                    if (_inputs.getRightHand.secondaryButtonPressed == false)
                    {
                        _rb.GetComponent<ConfigurableJoint>().angularXMotion = ConfigurableJointMotion.Locked;
                        _rb.GetComponent<ConfigurableJoint>().angularYMotion = ConfigurableJointMotion.Locked;
                        _rb.GetComponent<ConfigurableJoint>().angularZMotion = ConfigurableJointMotion.Locked;
                    }
                    #endregion
                }
                else
                {
                    DestroyJoint(collider.gameObject);
                }
            }
            #endregion

            #region Climbing
            if (collider.layer == LayerMask.NameToLayer("Climbable"))
            {
                prePos = _inputs.getLeftXRController.transform.localPosition;
                if (gripHold == true)
                {
                    #region Grab
                    
                    #endregion

                    dis += (prePos - _inputs.getLeftXRController.transform.localPosition);
                    Debug.Log("dis: " + dis);
                }

            }
            #endregion
        }
        #endregion
        

        private void DestroyJoint(GameObject go)
        {
            if (gripHold == false)
            {
                Destroy(go.GetComponent<Joint>());
                Debug.Log("Destroy");
            }
        }
    }
}