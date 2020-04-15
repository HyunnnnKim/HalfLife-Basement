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
        [Header("Movement")]
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float currentSpeed;
        public float CurrentSpeed { get { return currentSpeed; } set { currentSpeed = value; } }
        [SerializeField] private float moveForce;
        [SerializeField] private bool runKeyDown;

        [Header("Jump")]
        [SerializeField] private float jumpForce;
        [SerializeField] private bool jumpKeyDown;
        [SerializeField] private bool grounded = false;
        [SerializeField] private float distance;
        public float Distance { get { return distance; } set { distance = value; } }
        [SerializeField] private float noJumpHeight;

        [Header("Rotation")]
        [SerializeField] private rotateType selectedRotation;
        [SerializeField] private float rotationSensitivity = 70f;
        #endregion

        #region Private Variables
        private ControllerInput _inputs;
        public ControllerInput Inputs { get { return _inputs; } set { _inputs = value; } }
        private SnapTurnProvider _snap;
        public SnapTurnProvider Snap { get { return _snap; } set { _snap = value; } }
        private Rigidbody _rb;
        public Rigidbody RB { get { return _rb; } set { _rb = value; } }

        private Vector2 _position;
        public Vector2 Position { get { return _position; } set { _position = value; } }
        private Vector2 _rotation;
        public Vector2 Rotation { get { return _rotation; } set { _rotation = value; } }
        private Vector3 _lookDirection;
        public Vector3 LookDirect { get { return _lookDirection; } set { _lookDirection = value; } }
        private Vector3 _controllerDirection;
        public Vector3 ConDirect { get { return _controllerDirection; } set { _controllerDirection = value; } }
        private Vector3 _groundPoint;
        public Vector3 GroundPoint { get { return _groundPoint; } set { _groundPoint = value; } }

        private float _targetSpeed;
        public float TargetSpeed { get { return _targetSpeed; } set { _targetSpeed = value; } }
        #endregion

        #region BuiltIn Methods
        private void Start()
        {
            #region Initializing Components
            Inputs = ControllerInput.Instance;
            RB = GetComponent<Rigidbody>();
            Snap = GetComponent<SnapTurnProvider>();
            #endregion
        }

        private void Update()
        {
            #region Controller Inputs
            Position = Inputs.getLeftHand.primary2DValue;
            Rotation = Inputs.getRightHand.primary2DValue;

            jumpKeyDown = Inputs.getRightHand.primary2DPressed;
            runKeyDown = Inputs.getLeftHand.primary2DPressed;

            LookDirect = new Vector3(Position.x, 0f, Position.y);
            ConDirect = new Vector3(0, Inputs.getLeftXRController.transform.eulerAngles.y, 0);
            #endregion
        }

        private void FixedUpdate()
        {
            #region Update Value
            currentSpeed = RB.velocity.magnitude;
            #endregion
            
            #region Movement
            CanJump(RB);
            Rotate();
            RbMove(RB, moveForce);
            RbJump(RB, jumpForce);
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
                    GroundPoint = _rayHit.point;
                }

                distance = Vector3.Distance(rigidbody.transform.position, GroundPoint);
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

                    transform.Rotate(Vector3.up * Rotation.x * rotationSensitivity * perc);
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
            if (Inputs.getLeftHand.primary2DValueState)
            {
                LookDirect = Quaternion.Euler(ConDirect) * LookDirect;
                TargetSpeed = runKeyDown ? runSpeed : walkSpeed;
                Vector3 movement = LookDirect * TargetSpeed;

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