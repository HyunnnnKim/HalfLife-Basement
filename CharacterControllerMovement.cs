using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using HalfLife.Input;

namespace HalfLife.Movement
{
    public class CharacterControllerMovement : LocomotionProvider
    {
        private ControllerInput controllerInput;
        private CharacterController cc;
        private GameObject head;

        [SerializeField] private float speed = 12f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpHeight = 3f;

        [SerializeField] private Vector2 position;

        

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
            }

            private void FixedUpdate()
            {
                PostionController();
                MoveCharacter();
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

            private void MoveCharacter()
            {
                Vector3 direction = new Vector3(position.x, 0f, position.y);
                Vector3 headRotation = new Vector3(0f, head.transform.eulerAngles.y, 0f);

                direction = Quaternion.Euler(headRotation) * direction;

                Vector3 movement = direction * speed;
                cc.Move(movement * Time.deltaTime);
            }
        #endregion
    }
}