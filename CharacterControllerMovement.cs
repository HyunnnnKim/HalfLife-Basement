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
        #region Serialized Variables
            [Header("Physics")]
            [SerializeField] private float speed = 12f;
            [SerializeField] private float gravity = -9.81f;
            [SerializeField] private float jumpHeight = 3f;
            [SerializeField] private float rotationSensitivity = 100f;

            [Header("Player Status")]
            [SerializeField] private Transform player;
            [SerializeField] private bool jumpKeyDown;
            [SerializeField] private Vector3 velocity;
            [SerializeField] private bool isGrounded = true;
        #endregion
        
        #region Non-Serialized Variables
            private ControllerInput _controllerInput;
            private CharacterController _cc;
            private GameObject _head;
            private Vector2 _position;
            private Vector2 _rotation;
            private Vector3 _groundPoint;
            private Vector3 _lookDirection;
            private Vector3 _targetDirection;

            private float h, v;
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
                _rotation = _controllerInput.getRightHand.secondary2DValue;
                jumpKeyDown = _controllerInput.getRightHand.primaryButtonPressed;

                // _lookDirection += new Vector3(_rotation.x, _rotation.y, 4096);


                h = UnityEngine.Input.GetAxis("Horizontal");
                v = UnityEngine.Input.GetAxis("Vertical");
            }

            private void FixedUpdate()
            {
                // Postion();
                Rotation();
                Move();
                // CanJump();
                // Jump();
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
                // Quaternion tartgetRotation = Quaternion.LookRotation(_lookDirection, Vector3.back);
                // player.rotation = Quaternion.Lerp(player.transform.rotation, tartgetRotation, Time.deltaTime);

                _lookDirection = new Vector3(h, 0f, v);

                Vector3 vec = _lookDirection.normalized;
                vec.x = Mathf.Round(vec.x);
                vec.z = Mathf.Round(vec.z);
                if (vec.sqrMagnitude > 0.1f)
                    _targetDirection = vec.normalized;
                player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.LookRotation(_targetDirection), Time.deltaTime * rotationSensitivity);
            }

            private void Move()
            {
                Vector3 direction = new Vector3(_position.x, 0f, _position.y);
                Vector3 headRotation = new Vector3(0f, _head.transform.eulerAngles.y, 0f);

                direction = Quaternion.Euler(headRotation) * direction;

                Vector3 movement = direction * speed;
                _cc.Move(movement * Time.deltaTime);

                velocity.y += gravity * Time.deltaTime;
                _cc.Move(velocity * Time.deltaTime);
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