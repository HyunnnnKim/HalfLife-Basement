using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRcustom;

public class Climbing : MonoBehaviour
{
    enum ClimbingState
    {
        LeftHand,
        RightHand,
        BothHand,
        None
    }

    static ClimbingState climbingState = ClimbingState.None;

    ClimbingState thisHand;

    GameObject controller;
    Vector3 grabPoint;
    Vector3 XRRigRBPoint;

    GameObject climbaleObject;
    PlayerInput playerInput;

    GameObject XRRigRB;

    [SerializeField]
    private VRDviceNode hand;
    // Start is called before the first frame update
    void Start()
    {
        switch (hand)
        {
            case VRDviceNode.LeftHand:
            default:
                controller = GameObject.Find("LeftHand Controller");
                thisHand = ClimbingState.LeftHand;
                break;
            case VRDviceNode.RightHand:
                controller = GameObject.Find("RightHand Controller");
                thisHand = ClimbingState.RightHand;
                break;
        }
        XRRigRB = GameObject.Find("XR Rig with RigidBody");
        playerInput = PlayerInput.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (climbaleObject != null && playerInput.IsButtonPushed(ButtonName.grip, hand))
        {
            Locomotion_RigidBody.isClimbing = true;
            XRRigRB.GetComponent<Rigidbody>().isKinematic = true;
            XRRigRB.GetComponent<Rigidbody>().useGravity = false;
            grabPoint = controller.transform.localPosition;
            Debug.Log(Mathf.Atan2(grabPoint.x, grabPoint.z) * Mathf.Rad2Deg);
            XRRigRBPoint = XRRigRB.transform.localPosition;

            if (climbingState != thisHand && climbingState != ClimbingState.None)
            {
                climbingState = ClimbingState.BothHand;
            }
            else if(climbingState == ClimbingState.None)
            {
                climbingState = thisHand;
            }
        }
        if (playerInput.IsButtonReleased(ButtonName.grip, hand))
        {
            if (climbingState == ClimbingState.BothHand)
            {
                climbingState = thisHand == ClimbingState.LeftHand ? ClimbingState.RightHand : ClimbingState.LeftHand;
            }
            else if (climbingState != ClimbingState.None)
            {
                Locomotion_RigidBody.isClimbing = false;
                climbingState = ClimbingState.None;
                XRRigRB.GetComponent<Rigidbody>().isKinematic = false;
                XRRigRB.GetComponent<Rigidbody>().useGravity = true;
            }
        }
        
        moveXRRigRB();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!playerInput.GetHandInputData(hand).commonButtonStatus.gripButton && !other.gameObject.GetComponent<Rigidbody>())
        {
            climbaleObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        climbaleObject = null;
    }

    private void ReleaseHand()
    {
        climbingState = thisHand == ClimbingState.LeftHand ? ClimbingState.RightHand : ClimbingState.LeftHand;
    }

    private void moveXRRigRB()
    {
        if (climbingState == ClimbingState.BothHand)
        {

        }
        else if (climbingState == thisHand)
        {
            Vector3 offset = grabPoint - controller.transform.localPosition;
            Quaternion rotation = Quaternion.Euler(0f, XRRigRB.transform.localRotation.eulerAngles.y, 0f);
            Vector3 rotatedOffset = rotation * offset;
            XRRigRB.transform.localPosition = XRRigRBPoint + rotatedOffset;
        }
    }

}
