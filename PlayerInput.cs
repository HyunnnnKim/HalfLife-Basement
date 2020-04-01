using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

//https://docs.unity3d.com/Manual/xr_input.html
//https://docs.unity3d.com/ScriptReference/XR.CommonUsages.html

namespace VRcustom
{
    public enum VRControllerNode
    {
        RightHand = 0,
        leftHand = 1
    }

    [DisallowMultipleComponent, AddComponentMenu("Custom/VR Input")]
    public class PlayerInput : Singleton<PlayerInput>
    {
        protected PlayerInput() { }

        List<InputDevice> leftHandDevices = new List<InputDevice>();
        List<InputDevice> rightHandDevices = new List<InputDevice>();
        List<InputDevice> headDevices = new List<InputDevice>();
        InputDevice headDevice;
        InputDevice leftHandDevice;
        InputDevice rightHandDevice;

        public struct CommonButtonStatus
        {
            public bool primaryButton;
            public bool thumbButton;
            public bool thumbTouch;
            public bool gripButton;
            public bool triggerButton;
        }
        public struct CommonAxisStatus
        {
            public Vector2 thumb2DAxis;
            public float triggerAxis;
            public float gripAxis;
        }
        public struct OtherButtonStatus
        {
            public bool primaryTouch;
            public bool secondaryButton;
            public bool secondaryTouch;
            public bool userPresenceButton;
            public bool menuButton;
        }
        public struct OtherAxisStatus
        {
            public Vector2 secondary2DAxis;
            public float batteryLevelAxis;
        }
        public struct WhichHand
        {
            public CommonButtonStatus commonButtonStatus;
            public CommonAxisStatus commonAxisStatus;
            public OtherButtonStatus otherButtonStatus;
            public OtherAxisStatus otherAxisStatus;
        }

        private WhichHand leftHand;
        private WhichHand rightHand;

        void Start()
        {
            FindDevicesAtXRNode();
        }

        void Update()
        {
            if(!leftHandDevice.isValid || !rightHandDevice.isValid) FindDevicesAtXRNode();
            UpdateInput(ref leftHandDevice, ref leftHand);
            UpdateInput(ref rightHandDevice, ref rightHand);
        }

        void FindDevicesAtXRNode()
        {
            InputDevices.GetDevicesAtXRNode(XRNode.Head, headDevices);
            InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
            InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);

            CheckAndAssignDevice(ref headDevice, ref headDevices);
            CheckAndAssignDevice(ref leftHandDevice, ref leftHandDevices);
            CheckAndAssignDevice(ref rightHandDevice, ref rightHandDevices);
        }

        //TODO : Think how to proceed when devices count is zero or higher than one. And write some code.
        void CheckAndAssignDevice(ref InputDevice device, ref List<InputDevice> devices)
        {
            if (devices.Count == 1) //Fine
            {
                device = devices[0];
                Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.role.ToString()));
            }
            else if (headDevices.Count > 1)//WTF do you plug devices more than one?
            {
                Debug.Log("Found more than one device!");
            }
            else if (headDevices.Count == 0)//plug your device
            {
                Debug.Log("found no device!");
            }
        }

        void UpdateInput(ref InputDevice device, ref WhichHand hand)
        {
            //CommonButtonStatus
            device.TryGetFeatureValue(CommonUsages.gripButton, out hand.commonButtonStatus.gripButton);
            device.TryGetFeatureValue(CommonUsages.primaryButton, out hand.commonButtonStatus.primaryButton);
            device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out hand.commonButtonStatus.thumbButton);
            device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out hand.commonButtonStatus.thumbTouch);
            device.TryGetFeatureValue(CommonUsages.triggerButton, out hand.commonButtonStatus.triggerButton);

            //CommonAxisStatus
            device.TryGetFeatureValue(CommonUsages.grip, out hand.commonAxisStatus.gripAxis);
            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out hand.commonAxisStatus.thumb2DAxis);
            device.TryGetFeatureValue(CommonUsages.trigger, out hand.commonAxisStatus.triggerAxis);

            //OtherButtonStatus
            device.TryGetFeatureValue(CommonUsages.menuButton, out hand.otherButtonStatus.menuButton);
            device.TryGetFeatureValue(CommonUsages.primaryTouch, out hand.otherButtonStatus.primaryTouch);
            device.TryGetFeatureValue(CommonUsages.secondaryButton, out hand.otherButtonStatus.secondaryButton);
            device.TryGetFeatureValue(CommonUsages.secondaryTouch, out hand.otherButtonStatus.secondaryTouch);
            device.TryGetFeatureValue(CommonUsages.userPresence, out hand.otherButtonStatus.userPresenceButton);

            //OtherAxisStatus
            device.TryGetFeatureValue(CommonUsages.batteryLevel, out hand.otherAxisStatus.batteryLevelAxis);
            device.TryGetFeatureValue(CommonUsages.secondary2DAxis, out hand.otherAxisStatus.secondary2DAxis);
        }

        public WhichHand GetLeftHandInputData()
        {
            return leftHand;
        }
        public WhichHand GetRightHandInputData()
        {
            return rightHand;
        }
        public InputDevice GetVRControllerDevice(VRControllerNode pickLeftOrRight)
        {
            switch (pickLeftOrRight)
            {
                case VRControllerNode.leftHand:
                default:
                    return leftHandDevice;
                case VRControllerNode.RightHand:
                    return rightHandDevice;
            }
        }

        /// <summary>Play a haptic impulse on the controller if one is available</summary>
        /// <param name="amplitude">Amplitude (from 0.0 to 1.0) to play impulse at.</param>
        /// <param name="duration">Duration (in seconds) to play haptic impulse.</param>
        public bool SendHapticImpulse(VRControllerNode pickLeftOrRight, float amplitude, float duration)
        {
            HapticCapabilities capabilities;
            InputDevice controller = GetVRControllerDevice(pickLeftOrRight);
            if (controller.TryGetHapticCapabilities(out capabilities) && capabilities.supportsImpulse)
            {
                controller.StopHaptics();
                return controller.SendHapticImpulse(0, amplitude, duration);
            }
            return false;
        }
        public bool StopHapticImpulse(VRControllerNode pickLeftOrRight)
        {
            HapticCapabilities capabilities;
            InputDevice controller = GetVRControllerDevice(pickLeftOrRight);
            if (controller.TryGetHapticCapabilities(out capabilities) && capabilities.supportsImpulse)
            {
                controller.StopHaptics();
            }
            return false;
        }
    }
}