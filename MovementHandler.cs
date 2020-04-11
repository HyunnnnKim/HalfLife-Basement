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
                public enum movementType
                {
                    RigidBody,
                    CharacterController
                }
                public movementType SelectedMovement { get { return selectedBody; } set { selectedBody = value; } }

                public enum rotateType
                {
                    Snapturn,
                    SmoothRotation,
                    HeadTurn
                }
                public rotateType SelectedRotation { get { return selectedRotation; } set { selectedRotation = value; } }
            #endregion

            #region Serialized Variables
                [Header("Player Body")]
                [SerializeField] private Rigidbody _rb;
                [SerializeField] private movementType selectedBody = movementType.RigidBody;
                [SerializeField] private rotateType selectedRotation;

                [Header("Movement")]
                [SerializeField] private float walkSpeed = 5f;
                [SerializeField] private float runSpeed = 10f;
                [SerializeField] private float currentSpeed = 0f;
                public float CurrentSpeed { get { return currentSpeed; } set { currentSpeed = value; } }
                [SerializeField] private bool runKeyDown;

                [Header("Jump")]
                [SerializeField] private bool grounded = true;
                [SerializeField] private float distance;
                public float Distance { get { return distance; } set { distance = value; } }
                [SerializeField] private float noJumpHeight;
                [SerializeField] private bool jumpKeyDown;

                [Header("Rotation")]
                [SerializeField] private float rotationSensitivity = 70f;

                [Header("RigidBody")]
                [SerializeField] private float jumpForce = 30000f;
                [SerializeField] private ForceMode forceMode;

                [Header("Character Controller")]
                [SerializeField] private float gravity = -9.81f;
                [SerializeField] private float jumpHeight = 3f;

                [SerializeField] private Vector3 velocity;
            #endregion

            #region Private Variables
                private ControllerInput _controllerInput;
                private SnapTurnProvider _snap;
                private CharacterController _cc;
                private GameObject _head;

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
                _rb = GetComponentInChildren<Rigidbody>();
                _cc = GetComponent<CharacterController>();
                _head = GetComponent<XRRig>().cameraGameObject;
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
                #region Movement
                    Postion();
                    SetSpeed();
                    CanJump();
                    Rotate();

                    switch (selectedBody)
                    {
                        case movementType.RigidBody:
                            //Debug.Log("Selected Body: RigidBody");
                            noJumpHeight = 2f;
                            
                            RbMove();
                            RbJump();
                            break;

                        case movementType.CharacterController:
                            //Debug.Log("Selected Body: Character Controller");
                            noJumpHeight = 1f;

                            CcMove();
                            CcJump();
                            break;

                        default:
                            break;
                    }
                #endregion
            }
        #endregion

        #region CUSTOM METHODS
            #region Common Methods
                private void SetSpeed()
                {
                    if(runKeyDown)
                        currentSpeed = runSpeed;
                    else
                        currentSpeed = walkSpeed;
                }

                private void CanJump()
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 10f, Color.blue);
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit _rayHit, Mathf.Infinity))
                    {
                        if (String.Compare(_rayHit.collider.tag, "ground", StringComparison.Ordinal) == 0) /* StringComparison.Ordinal looks purely at the raw byte(s) that represent the character. */
                        {
                            _groundPoint = _rayHit.point;
                        }

                        distance = Vector3.Distance(transform.position, _groundPoint);
                        Debug.Log("transform: " + transform.position + " Distance: " + distance);

                        if (distance > noJumpHeight)
                            grounded = false;
                        else
                            grounded = true;
                    }
                }

                private void Postion()
                {
                    float headHeight = Mathf.Clamp(_head.transform.localPosition.y, 1f, 2f);
                    _cc.height = headHeight;

                    Vector3 newCenter = Vector3.zero;
                    newCenter.y = _cc.height / 2f;
                    newCenter.y += _cc.skinWidth;

                    newCenter.x = _head.transform.localPosition.x;
                    newCenter.z = _head.transform.localPosition.z;

                    _cc.center = newCenter;
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
                private void RbMove()
                {
                    _lookDirection = Quaternion.Euler(_controllerDirection) * _lookDirection;
                    _rb.MovePosition(transform.position + _lookDirection * Time.deltaTime * currentSpeed); /* Parameters of MovePosition() >>> transform.position + transform.forward * Time.deltaTime */
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

            #region Character Controller Movement
                private void CcMove()
                {
                    _cc.Move(_lookDirection * currentSpeed * Time.deltaTime);

                    velocity.y += gravity * Time.deltaTime;
                    _cc.Move(velocity * Time.deltaTime);
                }

                private void CcJump()
                {
                    if(jumpKeyDown && grounded)
                    {
                        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    }
                }
            #endregion
        #endregion
    }
}