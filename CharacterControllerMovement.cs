using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using HalfLife.Input;

namespace HalfLife.Movement
{
    public class CharacterControllerMovement : LocomotionProvider
    {
        #region Non-Serialized Variables
            private ControllerInput _controllerInput;
            private CharacterController _cc;
            private GameObject _head;
            private Vector2 _position;
            private Vector2 _rotation;
            private Vector3 _groundPoint;
            private Vector3 _lookDirection;
        #endregion

        #region Public Variables
            public enum rotateType
            {
                Snapturn,
                SmoothRotation,
                HeadTurn
            }
        #endregion

        #region Serialized Variables

            [SerializeField] private Transform player;

            [Header("Movement")]
            [SerializeField] private float walkSpeed = 5f;
            [SerializeField] private float runSpeed = 10f;
            [SerializeField] private float currentSpeed = 0f;
            [SerializeField] private bool runKeyDown;

            [Header("Jump")]
            [SerializeField] private float gravity = -9.81f;
            [SerializeField] private float jumpHeight = 3f;
            [SerializeField] private bool jumpKeyDown;

            [SerializeField] private float rotationSensitivity = 70f;
            [SerializeField] private Vector3 velocity;
            [SerializeField] private bool isGrounded = true;
            [SerializeField] private rotateType selectedRotation;
            public rotateType SelectedRotation { get { return selectedRotation; } set { selectedRotation = value; } }
        #endregion

        #region BuiltIn Methods
            protected override void Awake()
            {
                _controllerInput = ControllerInput.Instance;
                _cc = GetComponent<CharacterController>();
                _head = GetComponent<XRRig>().cameraGameObject;
            }

            private void Start()
            {
                Postion();
            }

            void Update()
            {
                _position = _controllerInput.getLeftHand.primary2DValue;
                _rotation = _controllerInput.getRightHand.primary2DValue;
                _lookDirection = transform.right * _position.x + transform.forward * _position.y;
                jumpKeyDown = _controllerInput.getRightHand.primary2DPressed;
                runKeyDown = _controllerInput.getLeftHand.primary2DPressed;
            }

            private void FixedUpdate()
            {
                Postion();
                Rotation();
                Move();
                CanJump();
                Jump();
            }
        #endregion

        #region Custom Methods
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

            private void Rotation()
            {
                switch (selectedRotation)
                {
                    case rotateType.Snapturn:
                        
                        break;

                    case rotateType.SmoothRotation:
                        float lerpTime = 1f;
                        float currentLerpTime = 0;

                        currentLerpTime += Time.deltaTime;
                        if (currentLerpTime > lerpTime)
                        {
                            currentLerpTime = lerpTime;
                        }
                        float perc = currentLerpTime / lerpTime;
                        
                        player.Rotate(Vector3.up * _rotation.x * rotationSensitivity * perc);
                        break;
                    
                    default:
                        break;
                }
            }

            private void Move()
            {
                if(runKeyDown)
                    currentSpeed = runSpeed;
                else
                    currentSpeed = walkSpeed;
                _cc.Move(_lookDirection * currentSpeed * Time.deltaTime);

                velocity.y += gravity * Time.deltaTime;
                _cc.Move(velocity * Time.deltaTime);
            }
            
            private void CurrentSpeed()
            {

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

                    var distance = Vector3.Distance(transform.position, _groundPoint);
                    Debug.Log("transform: " + transform.position + " Distance: " + distance);
                    if (distance > 1f)
                        isGrounded = false;
                    else
                        isGrounded = true;
                }
            }

            private void Jump()
            {
                if(jumpKeyDown && isGrounded)
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }
        #endregion
    }
}