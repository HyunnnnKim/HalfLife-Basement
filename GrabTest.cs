using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRcustom;

public class GrabTest : MonoBehaviour
{
    GameObject grabableObject;
    FixedJoint grab;
    PlayerInput playerInput;

    [SerializeField]
    private VRDviceNode hand;
    // Start is called before the first frame update
    void Start()
    {
        grab = GetComponent<FixedJoint>();
        grab.connectedBody = null;
        playerInput = PlayerInput.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabableObject != null && playerInput.IsButtonPushed(ButtonName.grip, hand))
        {
            grab.connectedBody = grabableObject.GetComponent<Rigidbody>();
        }
        if (playerInput.IsButtonReleased(ButtonName.grip, hand))
        {
            grab.connectedBody = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other);
        if (!playerInput.GetHandInputData(hand).commonButtonStatus.gripButton)
        {
            grabableObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        grabableObject = null;
    }
}
