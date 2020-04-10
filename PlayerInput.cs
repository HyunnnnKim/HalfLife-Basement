using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

//https://docs.unity3d.com/Manual/xr_input.html
//https://docs.unity3d.com/ScriptReference/XR.CommonUsages.html

namespace VRcustom
{
    public enum VRDviceNode
    {
        RightHand,
        LeftHand,
        Head
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

        public struct DeviceTrackingState
        {
            public Vector3 Acceleration;
            public Vector3 AngularAcceleration;
            public Vector3 AngularVelocity;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
        }

        private DeviceTrackingState head_TrackingState;
        private DeviceTrackingState hand_L_TrackingState;
        private DeviceTrackingState hand_R_TrackingState;

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
        public struct HandController
        {
            public CommonButtonStatus commonButtonStatus;
            public CommonAxisStatus commonAxisStatus;
            public OtherButtonStatus otherButtonStatus;
            public OtherAxisStatus otherAxisStatus;
        }

        private HandController leftHand;
        private HandController rightHand;


        void Start()
        {
            FindDevicesAtXRNode();
        }

        void Update()
        {
            if (!headDevice.isValid)
            {
                InputDevices.GetDevicesAtXRNode(XRNode.Head, headDevices);
                CheckAndAssignDevice(ref headDevice, ref headDevices);
            }
            if (!leftHandDevice.isValid) {
                InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
                CheckAndAssignDevice(ref leftHandDevice, ref leftHandDevices);
            }
            if (!rightHandDevice.isValid)
            {
                InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
                CheckAndAssignDevice(ref rightHandDevice, ref rightHandDevices);
            }
            UpdateInput(leftHandDevice, ref leftHand);
            UpdateInput(rightHandDevice, ref rightHand);
            UpdateTrackingState(headDevice, ref head_TrackingState);
            UpdateTrackingState(leftHandDevice, ref hand_L_TrackingState);
            UpdateTrackingState(rightHandDevice, ref hand_R_TrackingState);
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
                Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.characteristics.ToString()));
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

        void UpdateInput(InputDevice device, ref HandController handController)
        {
            //CommonButtonStatus
            device.TryGetFeatureValue(CommonUsages.gripButton, out handController.commonButtonStatus.gripButton);
            device.TryGetFeatureValue(CommonUsages.primaryButton, out handController.commonButtonStatus.primaryButton);
            device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out handController.commonButtonStatus.thumbButton);
            device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out handController.commonButtonStatus.thumbTouch);
            device.TryGetFeatureValue(CommonUsages.triggerButton, out handController.commonButtonStatus.triggerButton);

            //CommonAxisStatus
            device.TryGetFeatureValue(CommonUsages.grip, out handController.commonAxisStatus.gripAxis);
            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out handController.commonAxisStatus.thumb2DAxis);
            device.TryGetFeatureValue(CommonUsages.trigger, out handController.commonAxisStatus.triggerAxis);

            //OtherButtonStatus
            device.TryGetFeatureValue(CommonUsages.menuButton, out handController.otherButtonStatus.menuButton);
            device.TryGetFeatureValue(CommonUsages.primaryTouch, out handController.otherButtonStatus.primaryTouch);
            device.TryGetFeatureValue(CommonUsages.secondaryButton, out handController.otherButtonStatus.secondaryButton);
            device.TryGetFeatureValue(CommonUsages.secondaryTouch, out handController.otherButtonStatus.secondaryTouch);
            device.TryGetFeatureValue(CommonUsages.userPresence, out handController.otherButtonStatus.userPresenceButton);

            //OtherAxisStatus
            device.TryGetFeatureValue(CommonUsages.batteryLevel, out handController.otherAxisStatus.batteryLevelAxis);
            device.TryGetFeatureValue(CommonUsages.secondary2DAxis, out handController.otherAxisStatus.secondary2DAxis);
        }

        void UpdateTrackingState(InputDevice device, ref DeviceTrackingState deviceTrackingState)
        {
            device.TryGetFeatureValue(CommonUsages.deviceAcceleration, out deviceTrackingState.Acceleration);
            device.TryGetFeatureValue(CommonUsages.deviceAngularAcceleration, out deviceTrackingState.AngularAcceleration);
            device.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out deviceTrackingState.AngularVelocity);
            device.TryGetFeatureValue(CommonUsages.devicePosition, out deviceTrackingState.Position);
            device.TryGetFeatureValue(CommonUsages.deviceRotation, out deviceTrackingState.Rotation);
            device.TryGetFeatureValue(CommonUsages.deviceVelocity, out deviceTrackingState.Velocity);
        }

        public HandController GetLeftHandInputData()
        {
            return leftHand;
        }
        public HandController GetRightHandInputData()
        {
            return rightHand;
        }
        public DeviceTrackingState GetDeviceTrackingStateData(VRDviceNode node)
        {
            switch (node)
            {
                case VRDviceNode.Head:
                default:
                    return head_TrackingState;
                case VRDviceNode.LeftHand:
                    return hand_L_TrackingState;
                case VRDviceNode.RightHand:
                    return hand_R_TrackingState;
            }
        }
        public InputDevice GetVRDevice(VRDviceNode node)
        {
            switch (node)
            {
                case VRDviceNode.LeftHand:
                default:
                    return leftHandDevice;
                case VRDviceNode.RightHand:
                    return rightHandDevice;
                case VRDviceNode.Head:
                    return headDevice;
            }
        }

        /// <summary>Play a haptic impulse on the controller if one is available</summary>
        /// <param name="amplitude">Amplitude (from 0.0 to 1.0) to play impulse at.</param>
        /// <param name="duration">Duration (in seconds) to play haptic impulse.</param>
        public bool SendHapticImpulse(VRDviceNode node, float amplitude, float duration)
        {
            if (node == VRDviceNode.Head) return false;
            HapticCapabilities capabilities;
            InputDevice controller = GetVRDevice(node);
            if (controller.TryGetHapticCapabilities(out capabilities) && capabilities.supportsImpulse)
            {
                controller.StopHaptics();
                return controller.SendHapticImpulse(0, amplitude, duration);
            }
            return false;
        }
        public bool StopHapticImpulse(VRDviceNode node)
        {
            if (node == VRDviceNode.Head) return false;
            HapticCapabilities capabilities;
            InputDevice controller = GetVRDevice(node);
            if (controller.TryGetHapticCapabilities(out capabilities) && capabilities.supportsImpulse)
            {
                controller.StopHaptics();
            }
            return false;
        }
    }
}