using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using VRcustom;

public class VRInputLogInVR : MonoBehaviour
{
    // Start is called before the first frame update
    TextMeshPro textMeshPro;
    private PlayerInput playerInput;
    string text;
    void Start()
    {
        playerInput = PlayerInput.Instance;
        textMeshPro = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        text = "-----COMMON BUTTON STATUS-----\n";
        text += "--------------------------------<L>--------<R>\n";

        text += "primaryButton\t\t" + playerInput.GetLeftHandInputData().commonButtonStatus.primaryButton + "       " 
            + playerInput.GetRightHandInputData().commonButtonStatus.primaryButton + "\n";
        text += "thumbTouch\t\t" + playerInput.GetLeftHandInputData().commonButtonStatus.thumbTouch + "       "
            + playerInput.GetRightHandInputData().commonButtonStatus.thumbTouch + "\n";
        text += "thumbButton\t\t" + playerInput.GetLeftHandInputData().commonButtonStatus.thumbButton + "       "
            + playerInput.GetRightHandInputData().commonButtonStatus.thumbButton + "\n";
        text += "triggerButton\t\t" + playerInput.GetLeftHandInputData().commonButtonStatus.triggerButton + "       "
            + playerInput.GetRightHandInputData().commonButtonStatus.triggerButton + "\n";
        text += "gripButton\t\t\t" + playerInput.GetLeftHandInputData().commonButtonStatus.gripButton + "       "
            + playerInput.GetRightHandInputData().commonButtonStatus.gripButton + "\n";

        text += "-----COMMON AXIS STATUS-----\n";
        text += "--------------------------------<L>--------<R>\n";

        text += "gripAxis\t\t\t " + playerInput.GetLeftHandInputData().commonAxisStatus.gripAxis.ToString("F") + "         "
            + playerInput.GetRightHandInputData().commonAxisStatus.gripAxis.ToString("F") + "\n";
        text += "thumb2DAxis\t   " + playerInput.GetLeftHandInputData().commonAxisStatus.thumb2DAxis.ToString("F") + " "
            + playerInput.GetRightHandInputData().commonAxisStatus.thumb2DAxis.ToString("F") + "\n";
        text += "triggerAxis\t\t\t " + playerInput.GetLeftHandInputData().commonAxisStatus.triggerAxis.ToString("F") + "         "
            + playerInput.GetRightHandInputData().commonAxisStatus.triggerAxis.ToString("F") + "\n";
        
        text += "-----OTHER BUTTON STATUS-----\n";
        text += "--------------------------------<L>--------<R>\n";

        text += "menuButton\t\t" + playerInput.GetLeftHandInputData().otherButtonStatus.menuButton + "       "
            + playerInput.GetRightHandInputData().otherButtonStatus.menuButton + "\n";
        text += "primaryTouch\t\t" + playerInput.GetLeftHandInputData().otherButtonStatus.primaryTouch + "       "
            + playerInput.GetRightHandInputData().otherButtonStatus.primaryTouch + "\n";
        text += "secondaryTouch\t\t" + playerInput.GetLeftHandInputData().otherButtonStatus.secondaryTouch + "       "
            + playerInput.GetRightHandInputData().otherButtonStatus.secondaryTouch + "\n";
        text += "secondaryButton\t\t" + playerInput.GetLeftHandInputData().otherButtonStatus.secondaryButton + "       "
            + playerInput.GetRightHandInputData().otherButtonStatus.secondaryButton + "\n";
        text += "userPresenceButton\t" + playerInput.GetLeftHandInputData().otherButtonStatus.userPresenceButton + "       "
            + playerInput.GetRightHandInputData().otherButtonStatus.userPresenceButton + "\n";

        text += "-----OTHER AXIS STATUS-----\n";
        text += "--------------------------------<L>--------<R>\n";

        text += "batteryLevelAxis\t\t " + playerInput.GetLeftHandInputData().otherAxisStatus.batteryLevelAxis.ToString("F") + "         "
            + playerInput.GetRightHandInputData().otherAxisStatus.batteryLevelAxis.ToString("F") + "\n";
        text += "secondary2DAxis\t   " + playerInput.GetLeftHandInputData().otherAxisStatus.secondary2DAxis.ToString("F") + "  "
            + playerInput.GetRightHandInputData().otherAxisStatus.secondary2DAxis.ToString("F") + "\n";
        
        //Debug.Log(text);

        textMeshPro.text = text;
    }
}
