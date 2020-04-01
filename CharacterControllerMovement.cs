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

            [Header("Player Status")]
            [SerializeField] private bool jumpKeyDown;
            [SerializeField] private Vector3 velocity;
            [SerializeField] private bool isGrounded = true;
        #endregion
        
        #region Non-Serialized Variables
            private ControllerInput controllerInput;
            private CharacterController cc;
            private GameObject head;
            private Vector2 position;
            private Vector3 _groundPoint;

        #endregion

        #region BuiltIn Methods
            protected override void Awake()
            {
                controllerInput = ControllerInput.Instance;

                cc = GetComponent<CharacterController>();
                head = GetComponent<XRRig>().cameraGameObject;
            }

            private void Start()
            {
                PostionController();
            }

            void Update()
            {
                position = controllerInput.getLeftHand.primary2DValue;
                jumpKeyDown = controllerInput.getRightHand.primaryButtonPressed;
            }

            private void FixedUpdate()
            {
                PostionController();
                Move();
                CanJump();
                Jump();
            }
        #endregion

        #region Custom Methods
            private void PostionController()
            {
                float headHeight = Mathf.Clamp(head.transform.localPosition.y, 1f, 2f);
                cc.height = headHeight;

                Vector3 newCenter = Vector3.zero;
                newCenter.y = cc.height / 2f;
                newCenter.y += cc.skinWidth;

                newCenter.x = head.transform.localPosition.x;
                newCenter.z = head.transform.localPosition.z;

                cc.center = newCenter;
            }

            private void Move()
            {
                Vector3 direction = new Vector3(position.x, 0f, position.y);
                Vector3 headRotation = new Vector3(0f, head.transform.eulerAngles.y, 0f);

                direction = Quaternion.Euler(headRotation) * direction;

                Vector3 movement = direction * speed;
                cc.Move(movement * Time.deltaTime);

                velocity.y += gravity * Time.deltaTime;
                cc.Move(velocity * Time.deltaTime);
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