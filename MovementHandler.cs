using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using HalfLight.Input;

namespace HalfLight.Movement
{
    public class MovementHandler : MonoBehaviour
    {
        #region VARIABLES
            #region Public Variables
                public enum rotateType
                {
                    Snapturn,
                    SmoothRotation,
                    HeadTurn
                }
                public rotateType SelectedRotation { get { return selectedRotation; } set { selectedRotation = value; } }
            #endregion

            #region Serialized Variables
                [Header("Player")]
                [SerializeField] private Rigidbody _rb;
                [SerializeField] private rotateType selectedRotation;

                [Header("Movement")]
                [SerializeField] private float currentSpeed = 0f;
                public float CurrentSpeed { get { return currentSpeed; } set { currentSpeed = value; } }
                [SerializeField] private bool runKeyDown;

                [Header("Jump")]
                [SerializeField] private float jumpForce = 30000f;
                [SerializeField] private ForceMode forceMode;
                [SerializeField] private bool grounded = false;
                [SerializeField] private float distance;
                public float Distance { get { return distance; } set { distance = value; } }
                [SerializeField] private float noJumpHeight;
                [SerializeField] private bool jumpKeyDown;

                [Header("Rotation")]
                [SerializeField] private float rotationSensitivity = 70f;
            #endregion

            #region Private Variables
                private ControllerInput _controllerInput;
                private SnapTurnProvider _snap;
                private Vector2 _position;
                private Vector2 _rotation;
                private Vector3 _lookDirection;
                private Vector3 _controllerDirection;
                private Vector3 _groundPoint;
            #endregion
        #endregion

        #region BUILTIN METHODS
            private void Start() {
                _controllerInput = ControllerInput.Instance;
                // _rb = GetComponent<Rigidbody>();
                _snap = GetComponent<SnapTurnProvider>();
            }

            private void Update() {
                #region Controller Inputs
                    _position = _controllerInput.getLeftHand.primary2DValue;
                    _rotation = _controllerInput.getRightHand.primary2DValue;

                    jumpKeyDown = _controllerInput.getRightHand.primary2DPressed;
                    runKeyDown = _controllerInput.getLeftHand.primary2DPressed;

                    // _lookDirection = transform.TransformDirection(_position.x, 0f, _position.y);
                    _position.x = Mathf.Clamp(_position.x, -90f, 90f);
                    _lookDirection = new Vector3(_position.x, 0f, _position.y);
                    _controllerDirection = new Vector3(0, _controllerInput.getLeftController.transform.eulerAngles.y, 0);
                #endregion
            }
            
            private void FixedUpdate() {
                currentSpeed = _rb.velocity.magnitude;
                #region Movement
                    SetSpeed();
                    CanJump();
                    Rotate();
                    RbMove(_rb, Vector3.forward * 10, 10f);
                    RbJump();
                #endregion
            }
        #endregion

        #region CUSTOM METHODS
            #region Common Methods
                private void SetSpeed()
                {
                    return;
                }

                private void CanJump()
                {
                    Debug.DrawRay(_rb.transform.position, _rb.transform.TransformDirection(Vector3.down) * 10f, Color.blue);
                    if (Physics.Raycast(_rb.transform.position, _rb.transform.TransformDirection(Vector3.down), out RaycastHit _rayHit, Mathf.Infinity))
                    {
                        if (String.Compare(_rayHit.collider.tag, "ground", StringComparison.Ordinal) == 0) /* StringComparison.Ordinal looks purely at the raw byte(s) that represent the character. */
                        {
                            _groundPoint = _rayHit.point;
                        }

                        distance = Vector3.Distance(_rb.transform.position, _groundPoint);
                        // Debug.Log("transform: " + _rb.transform.position + " Distance: " + distance);

                        if (distance > noJumpHeight)
                            grounded = false;
                        else
                            grounded = true;
                    }
                }

                private void Rotate()
                {
                    switch (selectedRotation)
                    {
                        case rotateType.Snapturn:
                            //Debug.Log("Selected Rotation: Snap");
                            break;

                        case rotateType.SmoothRotation:
                            //Debug.Log("Selected Rotation: Smooth");
                            float lerpTime = 1f;
                            float currentLerpTime = 0;

                            currentLerpTime += Time.deltaTime;
                            if (currentLerpTime > lerpTime)
                            {
                                currentLerpTime = lerpTime;
                            }
                            float perc = currentLerpTime / lerpTime;
                            
                            transform.Rotate(Vector3.up * _rotation.x * rotationSensitivity * perc);
                            break;
                        
                        default:
                            break;
                    }
                }
            #endregion
            
            /// <summary>
            /// Currently using controller based movement only on RigidBody.
            /// </summary>
            #region RigidBody Movement
                private void RbMove(Rigidbody rigidbody, Vector3 velocity, float force = 1, ForceMode mode = ForceMode.Force)
                {
                    // _lookDirection = Quaternion.Euler(_controllerDirection) * _lookDirection;
                    // _rb.MovePosition(transform.position + _lookDirection * Time.deltaTime * currentSpeed); /* Parameters of MovePosition() >>> transform.position + transform.forward * Time.deltaTime */

                    // if(force == 0 || velocity.magnitude == 0)
                    //     return;

                    // velocity = velocity + velocity.normalized * 0.2f * rigidbody.drag;
                    // force = Mathf.Clamp(force, -rigidbody.mass / Time.fixedDeltaTime, rigidbody.mass / Time.fixedDeltaTime);

                    // if(rigidbody.velocity.magnitude == 0)
                    // {
                    //     rigidbody.AddForce(velocity * force, mode);
                    // }
                    // else
                    // {
                    //     var velocityProjectedToTarget = (velocity.normalized * Vector3.Dot(velocity, rigidbody.velocity) / velocity.magnitude);
                    //     rigidbody.AddForce((velocity - velocityProjectedToTarget) * force, mode);
                    // }

                    Debug.Log(_controllerInput.getLeftHand.primary2DValue);
                    if(_controllerInput.getLeftHand.Primary2DValueState)
                    {
                        // rigidbody.AddForce(Vector3.forward * 1000f);
                        
                    }

                    // rigidbody.AddForce(Vector3.forward * 1000f);
                }

                private void RbJump()
                {
                    if(jumpKeyDown && grounded)
                    {
                        _rb.AddForce(jumpForce * _rb.mass * Time.deltaTime * Vector3.up, forceMode);
                        Debug.Log("Player jumping.");
                    }
                }     
            #endregion
        #endregion
    }
}