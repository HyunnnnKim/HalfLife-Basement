using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class PlayerInput : Singleton<PlayerInput>
{
    public XRNode xrNode = XRNode.LeftHand;
    public List<InputDevice> devices = new List<InputDevice>();

    public InputDevice device;

    public Vector2 position;

    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(xrNode, devices);
        device = devices.FirstOrDefault();
    }

    private void OnEnable()
    {
        if (!device.isValid)
        { 
            GetDevice();
        }
    }


    void Update()
    {
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out position);
    }

    public Vector2 GetInput()
    {
        return position;
    }
    public string Getname()
    {
        return device.name;
    }
}
