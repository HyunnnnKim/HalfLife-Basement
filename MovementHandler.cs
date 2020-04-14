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
        #region Public Variables
        public enum rotateType
        {
            Snapturn,
            SmoothRotation
        }
        public rotateType SelectedRotation { get { return selectedRotation; } set { selectedRotation = value; } }
        #endregion

        #region Serialized Variables
        [Header("Player")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private rotateType selectedRotation;

        [Header("Movement")]
        [SerializeField] private float walkSpeed;
        public float WalkSpeed { get { return walkSpeed; } set { walkSpeed = value; } }
        [SerializeField] private float runSpeed;
        public float RunSpeed { get { return runSpeed; } set { runSpeed = value; } }
        [SerializeField] private float currentSpeed;
        public float CurrentSpeed { get { return currentSpeed; } set { currentSpeed = value; } }
        [SerializeField] private float moveForce;
        [SerializeField] private bool runKeyDown;

        [Header("Jump")]
        [SerializeField] private float jumpForce;
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

        private float _targetSpeed;
        public float TargetSpeed { get { return _targetSpeed; } set { _targetSpeed = value; } }
        #endregion

        #region BuiltIn Methods
        private void Start()
        {
            #region Initializing Components
            _controllerInput = ControllerInput.Instance;
            _rb = GetComponent<Rigidbody>();
            _snap = GetComponent<SnapTurnProvider>();
            #endregion
        }

        private void Update()
        {
            #region Controller Inputs
            _position = _controllerInput.getLeftHand.primary2DValue;
            _rotation = _controllerInput.getRightHand.primary2DValue;

            jumpKeyDown = _controllerInput.getRightHand.primary2DPressed;
            runKeyDown = _controllerInput.getLeftHand.primary2DPressed;

            _lookDirection = new Vector3(_position.x, 0f, _position.y);
            _controllerDirection = new Vector3(0, _controllerInput.getLeftXRController.transform.eulerAngles.y, 0);
            #endregion
        }

        private void FixedUpdate()
        {
            #region Update Value
            currentSpeed = _rb.velocity.magnitude;
            #endregion
            
            #region Movement
            CanJump(_rb);
            Rotate();
            RbMove(_rb, moveForce);
            RbJump(_rb, jumpForce);
            #endregion
        }
        #endregion

        #region Custom Methods
        private void CanJump(Rigidbody rigidbody)
        {
            Debug.DrawRay(rigidbody.transform.position, rigidbody.transform.TransformDirection(Vector3.down) * 10f, Color.blue);
            if (Physics.Raycast(rigidbody.transform.position, rigidbody.transform.TransformDirection(Vector3.down), out RaycastHit _rayHit, Mathf.Infinity))
            {
                if (String.Compare(_rayHit.collider.tag, "ground", StringComparison.Ordinal) == 0) /* StringComparison.Ordinal looks purely at the raw byte(s) that represent the character. */
                {
                    _groundPoint = _rayHit.point;
                }

                distance = Vector3.Distance(rigidbody.transform.position, _groundPoint);
                // Debug.Log("transform: " + rigidbody.transform.position + " Distance: " + distance);

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

        /// <summary>
        /// AddForce accumilates over time is the problem that needs to be fixed.
        /// MovePosition teleports non kinematic objects, so I believe it is not the right choice for physical interaction.
        /// </summary>
        private void RbMove(Rigidbody rigidbody, float force, ForceMode mode = ForceMode.Force)
        {
            // _rb.MovePosition(transform.position + _lookDirection * Time.deltaTime * currentSpeed); /* Parameters of MovePosition() >>> transform.position + transform.forward * Time.deltaTime */
            if (_controllerInput.getLeftHand.primary2DValueState)
            {
                _lookDirection = Quaternion.Euler(_controllerDirection) * _lookDirection;
                _targetSpeed = runKeyDown ? runSpeed : walkSpeed;
                Vector3 movement = _lookDirection * _targetSpeed;

                rigidbody.AddForce(movement * force, mode);
            }
        }

        private void RbJump(Rigidbody rigidbody, float force, ForceMode mode = ForceMode.Impulse)
        {
            if (jumpKeyDown && grounded)
            {
                rigidbody.AddForce(force * rigidbody.mass * Time.deltaTime * Vector3.up, mode);
                // rigidbody.velocity += Vector3.up * Physics.gravity.y * (400f - 1) * Time.deltaTime;
            }
        }
        #endregion
    }
}