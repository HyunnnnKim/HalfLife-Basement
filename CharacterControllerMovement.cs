using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


namespace HalfLife.Movement
{
    public class CharacterControllerMovement : LocomotionProvider
    {
        

        [SerializeField] private Vector2 position;

        public List<XRController> controllers = null;

        private CharacterController cc = null;
        private GameObject head = null;

        public float speed = 1.0f;
        public float gravityMultiplier = 1.0f;
        protected override void Awake()
        {
 
        }

        private void Start()
        {
            cc = GetComponent<CharacterController>();
            head = GetComponent<XRRig>().cameraGameObject;
            PostionController();
        }

        private void Update()
        {
            PostionController();
            CheckForInput();
        }

        private void PostionController()
        {
            float headHeight = Mathf.Clamp(head.transform.localPosition.y, 1, 2);
            cc.height = headHeight;

            Vector3 newCenter = Vector3.zero;
            newCenter.y = cc.height / 2;
            newCenter.y += cc.skinWidth;

            newCenter.x = head.transform.localPosition.x;
            newCenter.z = head.transform.localPosition.z;

            cc.center = newCenter;
        }

        private void CheckForInput()
        {
            foreach (XRController controller in controllers)
            {
                if (controller.enableInputActions)
                {
                    CheckForMovement(controller.inputDevice);
                }
            }
        }

        private void CheckForMovement(InputDevice device)
        {
            if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out position))
            {
                StartMove(position);
            }
        }

        private void StartMove(Vector2 position)
        {
            Vector3 direction = new Vector3(position.x, 0, position.y);
            Vector3 headRotation = new Vector3(0, head.transform.eulerAngles.y, 0);

            direction = Quaternion.Euler(headRotation) * direction;

            Vector3 movement = direction * speed;
            cc.Move(movement * Time.deltaTime);
        }

        private void ApplyGravity()
        {
            Vector3 gravity = new Vector3(0, Physics.gravity.y * gravityMultiplier, 0);
            gravity.y *= Time.deltaTime;

            cc.Move(gravity * Time.deltaTime);
        }

        public Vector2 GetInput()
        {
            return position;
        }

    }
}