using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace HalfLife.Input
{
    public class ControllerInput : Singleton<ControllerInput>
    {
        #region Devices
            [Header("Controllers")]
            [SerializeField] private XRController leftController = null;
            [SerializeField] private XRController rightController = null;

            //[SerializeField] private XRNode liftXRNode = XRNode.LeftHand;
            //[SerializeField] private XRNode rightXRNode = XRNode.RightHand;

            //private List<InputDevice> devices = new List<InputDevice>();
            //private InputDevice leftDevice, rightDevice;
        #endregion

        #region Private Variables
            public struct InputValues
            {
                /* Primary2DAxis */
                private Vector2 _primary2DValue;
                public Vector2 primary2DValue { get { return _primary2DValue; } set { _primary2DValue = value; } }
                private bool _primary2DPressed;
                public bool primary2DPressed { get { return _primary2DPressed; } set { _primary2DPressed = value; } }
                private bool _primary2DTouchPressed;
                public bool primary2DTouchPressed { get { return _primary2DTouchPressed; } set { _primary2DTouchPressed = value; } }

                /* Secondary2DAxis */
                private Vector2 _secondary2DValue;
                public Vector2 secondary2DValue { get { return _secondary2DValue; } set { _secondary2DValue = value; } }
                private bool _secondary2DPressed;
                public bool secondary2DPressed { get { return _secondary2DPressed; } set { _secondary2DPressed = value; } }
                private bool _secondary2DTouchPressed;
                public bool secondary2DTouchPressed { get { return _secondary2DTouchPressed; } set { _secondary2DTouchPressed = value; } }

                /* Trigger */
                private float _triggerValue;
                public float triggerValue { get { return _triggerValue; } set { _triggerValue = value; } }
                private bool _triggerButtonPressed;
                public bool triggerButtonPressed { get { return _triggerButtonPressed; } set { _triggerButtonPressed = value; } }

                /* Grip */
                private float _gripValue;
                public float gripValue { get { return _gripValue; } set { _gripValue = value; } }
                private bool _gripButtonPressed;
                public bool gripButtonPressed { get { return _gripButtonPressed; } set { _gripButtonPressed = value; } }

                /* PrimaryButton */
                private bool _primaryButtonPressed;
                public bool primaryButtonPressed { get { return _primaryButtonPressed; } set { _primaryButtonPressed = value; } }
                private bool _primaryTouchPressed;
                public bool primaryTouchPressed { get { return _primaryTouchPressed; } set { _primaryTouchPressed = value; } }

                /* SecondaryButton */
                private bool _secondaryButtonPressed;
                public bool secondaryButtonPressed { get { return _secondaryButtonPressed; } set { _secondaryButtonPressed = value; } }
                private bool _secondaryTouchPressed;
                public bool secondaryTouchPressed { get { return _secondaryTouchPressed; } set { _secondaryTouchPressed = value; } }

                /* MenuButton */
                private bool _menuButtonPressed;
                public bool menuButtonPressed { get { return _menuButtonPressed; } set { _menuButtonPressed = value; } }
                
                /* BatteryLevel */
                private float _batteryLevelValue;
                public float batteryLevelValue { get { return _batteryLevelValue; } set { _batteryLevelValue = value; } }
                
                /* UserPresence */
                private bool _userPresence;
                public bool userPresence { get { return _userPresence; } set { _userPresence = value; } }
            }
        #endregion

        #region Serialized Private Variables
            [SerializeField] private InputValues leftHand;
            public InputValues getLeftHand { get { return leftHand; } }
            [SerializeField] private InputValues rightHand;
            public InputValues getRightHand { get { return rightHand; } }
        #endregion

        #region BuiltIn Methods
            private void Start()
            {
                //InputDevices.GetDevicesAtXRNode(liftXRNode, devices);
                //leftDevice = devices.FirstOrDefault();
                //InputDevices.GetDevicesAtXRNode(rightXRNode, devices);
                //rightDevice = devices.FirstOrDefault();
            }

            private void Update()
            {
                if (leftController.enableInputActions || rightController.enableInputActions)
                {
                    ReadInput(leftController.inputDevice, ref leftHand);
                    ReadInput(rightController.inputDevice, ref rightHand);
                }

                //ReadInput(leftDevice, ref leftHand);
                //ReadInput(rightDevice, ref rightHand);
            }
        #endregion

        #region Custom Methods
            private void ReadInput(InputDevice device, ref InputValues hand)
            {
                device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DValue);
                hand.primary2DValue = primary2DValue;
                device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool primary2DPressed);
                hand.primary2DPressed = primary2DPressed;
                device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out bool primary2DTouchPressed);
                hand.primary2DTouchPressed = primary2DTouchPressed;

                device.TryGetFeatureValue(CommonUsages.secondary2DAxis, out Vector2 secondary2DValue);
                hand.secondary2DValue = secondary2DValue;
                device.TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out bool secondary2DPressed);
                hand.secondary2DPressed = secondary2DPressed;
                device.TryGetFeatureValue(CommonUsages.secondary2DAxisTouch, out bool secondary2DTouchPressed);
                hand.secondary2DTouchPressed = secondary2DTouchPressed;

                device.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
                hand.triggerValue = triggerValue;
                device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButtonPressed);
                hand.triggerButtonPressed = triggerButtonPressed;

                device.TryGetFeatureValue(CommonUsages.grip, out float gripValue);
                hand.gripValue = gripValue;
                device.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButtonPressed);
                hand.gripButtonPressed = gripButtonPressed;

                device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonPressed);
                hand.primaryButtonPressed = primaryButtonPressed;
                device.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouchPressed);
                hand.primaryTouchPressed = primaryTouchPressed;

                device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonPressed);
                hand.secondaryButtonPressed = secondaryButtonPressed;
                device.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool secondaryTouchPressed);
                hand.secondaryTouchPressed = secondaryTouchPressed;

                device.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonPressed);
                hand.menuButtonPressed = menuButtonPressed;

                device.TryGetFeatureValue(CommonUsages.batteryLevel, out float batteryLevelValue);
                hand.batteryLevelValue = batteryLevelValue;
                device.TryGetFeatureValue(CommonUsages.userPresence, out bool userPresence);
                hand.userPresence = userPresence;
            }
        #endregion
    }
}