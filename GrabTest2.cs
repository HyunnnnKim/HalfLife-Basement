using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRcustom;

public class GrabTest2 : MonoBehaviour
{
    GameObject grabableObject;
    FixedJoint grab;
    Rigidbody rb;
    PlayerInput playerInput;

    [SerializeField]
    private VRDviceNode hand;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = PlayerInput.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabableObject != null && playerInput.IsButtonPushed(ButtonName.grip, hand))
        {
            grab = grabableObject.AddComponent<FixedJoint>();
            grab.connectedBody = rb;
        }
        if (playerInput.IsButtonReleased(ButtonName.grip, hand))
        {
            Destroy(grab);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (!playerInput.GetHandInputData(hand).commonButtonStatus.gripButton && other.gameObject.GetComponent<Rigidbody>())
        {
            grabableObject = other.gameObject;
        }
        else
        {
            grabableObject = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        grabableObject = null;
        //if(grab != null) Destroy(grab);
    }
}
