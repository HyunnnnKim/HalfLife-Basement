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
            Spring
        }

        public enum hand
        {
            leftHand,
            rightHand
        }
        #endregion

        #region Serialized Field
        [SerializeField] private GameObject playerBody;

        [SerializeField] private hand selectedHand;
        [SerializeField] private interactType selectedGrab;

        [SerializeField] private bool grabHold;
        #endregion

        #region Private Field
        private ControllerInput _inputs;
        private Rigidbody _rb;
        private LayerMask _isInteractable;

        private ConstraintSource constraintSource;

        public Vector3 prePos;
        public Vector3 dis;
        #endregion

        #region BuiltIn Methods
        private void Start()
        {
            _inputs = ControllerInput.Instance;
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            GetGrab(selectedHand);
        }
        #endregion

        #region Custom Methods
        private void OnTriggerStay(Collider collider)
        {
            #region Grabbing
            if (collider.gameObject.GetComponent<Rigidbody>() != null
                    && collider.gameObject.layer == LayerMask.NameToLayer("Grabable"))
            {
                if (grabHold == true)
                {
                    #region Create Joint
                    switch (selectedGrab)
                    {
                        case interactType.Fixed:
                            if (collider.gameObject.GetComponent<FixedJoint>() == null)
                            {
                                collider.gameObject.AddComponent<FixedJoint>();
                                collider.gameObject.GetComponent<FixedJoint>().connectedBody = _rb;
                            }
                            break;

                        case interactType.Spring:
                            if (collider.gameObject.GetComponent<SpringJoint>() == null)
                            {
                                collider.gameObject.AddComponent<SpringJoint>();
                                collider.gameObject.GetComponent<SpringJoint>().connectedBody = _rb;
                            }
                            break;
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
                else if (collider.gameObject.GetComponent<Joint>() != null)
                {
                    DestroyJoint(collider.gameObject);
                }
            }
            #endregion

            #region Climbing
            if (collider.gameObject.layer == LayerMask.NameToLayer("Climbable"))
            {
                prePos = _inputs.getLeftXRController.transform.position;
                if (grabHold == true)
                {
                    #region Grab
                    // var parentConstraint = collider.gameObject.AddComponent<ParentConstraint>();
                    // constraintSource.sourceTransform = _leftController.transform;
                    // constraintSource.weight = 1f;

                    // parentConstraint.AddSource(constraintSource);
                    // parentConstraint.weight = 1f;
                    // parentConstraint.constraintActive = true;
                    // parentConstraint.locked = true;
                    #endregion

                    collider.gameObject.AddComponent<FixedJoint>();
                    collider.gameObject.GetComponent<FixedJoint>().connectedBody = _rb;

                    dis += (prePos - _inputs.getLeftXRController.transform.position);
                    Debug.Log("dis: " + dis);
                }
                else if (collider.gameObject.GetComponent<Joint>() != null)
                {
                    prePos = _inputs.getLeftXRController.transform.position;
                    DestroyJoint(collider.gameObject);
                }
            }
            #endregion
        }

        private void DestroyJoint(GameObject go)
        {
            if (grabHold == false)
            {
                Destroy(go.GetComponent<Joint>());
                Debug.Log("Destroy");
            }
        }

        private void GetGrab(hand selectedGrab)
        {
            switch (selectedHand)
            {
                case hand.leftHand:
                    grabHold = _inputs.getLeftHand.gripButtonPressed;
                    break;
                case hand.rightHand:
                    grabHold = _inputs.getRightHand.gripButtonPressed;
                    break;
            }
        }
        #endregion
    }
}