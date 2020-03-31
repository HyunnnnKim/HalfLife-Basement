using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

//https://docs.unity3d.com/Manual/xr_input.html
//https://docs.unity3d.com/ScriptReference/XR.CommonUsages.html


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
        public bool primary;
        public bool thumb;
        public bool thumbTouch;
        public bool grip;
        public bool trigger;
    }
    public struct CommonAxisStatus
    {
        public Vector2 thumb;
        public float trigger;
        public float grip;
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
        UpdateInput(ref leftHandDevice, ref leftHand);
        UpdateInput(ref rightHandDevice, ref rightHand);
    }

    bool FindDevicesAtXRNode()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.Head, headDevices);
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);

        return (CheckAndAssignDevice(ref headDevice, ref headDevices) == 1)
            && (CheckAndAssignDevice(ref leftHandDevice, ref leftHandDevices) == 1)
            && (CheckAndAssignDevice(ref rightHandDevice, ref rightHandDevices) == 1);
    }

    //TODO : Think how to proceed when devices count is zero or higher than one. And write some code.
    int CheckAndAssignDevice(ref InputDevice device, ref List<InputDevice> devices)
    {
        if (devices.Count == 1) //Fine
        {
            device = devices[0];
            Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.role.ToString()));
            return 1;
        }
        else if (headDevices.Count > 1)//WTF do you plug devices more than one?
        {
            Debug.Log("Found more than one device!");
            return 2;
        }
        else if (headDevices.Count == 0)//plug your device
        {
            Debug.Log("found no device!");
            return 0;
        }
        return -1;
    }

    void UpdateInput(ref InputDevice device, ref WhichHand hand)
    {
        //CommonButtonStatus
        device.TryGetFeatureValue(CommonUsages.gripButton, out hand.commonButtonStatus.grip);
        device.TryGetFeatureValue(CommonUsages.primaryButton, out hand.commonButtonStatus.primary);
        device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out hand.commonButtonStatus.thumb);
        device.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out hand.commonButtonStatus.thumbTouch);
        device.TryGetFeatureValue(CommonUsages.triggerButton, out hand.commonButtonStatus.trigger);

        //CommonAxisStatus
        device.TryGetFeatureValue(CommonUsages.grip, out hand.commonAxisStatus.grip);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out hand.commonAxisStatus.thumb);
        device.TryGetFeatureValue(CommonUsages.trigger, out hand.commonAxisStatus.trigger);

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

    public WhichHand GetLeftHand()
    {
        return leftHand;
    }
    public WhichHand GetRightHand()
    {
        return rightHand;
    }
}
