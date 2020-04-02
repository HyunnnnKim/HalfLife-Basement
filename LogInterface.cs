using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using HalfLife.Movement;

public class LogInterface : MonoBehaviour
{
    [SerializeField] private Button smoothBtn;
    [SerializeField] private Button snapBtn;

    [SerializeField] private GameObject cube;
    [SerializeField] private CharacterControllerMovement movementScript;
    [SerializeField] private LogText logScript;

    private SnapTurnProvider _snapTurnScript;

    private void Awake() {
        
    }

    void Start()
    {
        smoothBtn.onClick.AddListener(() =>
        {
            cube.gameObject.GetComponent<Rigidbody>().AddForce(3000f * Vector3.up);

            movementScript.GetComponent<CharacterControllerMovement>();
            movementScript.selectedRotation = CharacterControllerMovement.rotateType.SmoothRotation;

            movementScript.GetComponentInChildren<SnapTurnProvider>().enabled = false;

            if(movementScript.GetComponentInChildren<SnapTurnProvider>().enabled == false)
            {
                logScript.GetComponent<LogText>().textval += "snap off";
            }
            else
            {
                logScript.GetComponent<LogText>().textval += "nope";
            }
        });

        snapBtn.onClick.AddListener(() =>
        {
            cube.gameObject.GetComponent<Rigidbody>().AddForce(3000f * Vector3.right);

            movementScript.GetComponent<CharacterControllerMovement>();
            movementScript.selectedRotation = CharacterControllerMovement.rotateType.Snapturn;

            movementScript.GetComponentInChildren<SnapTurnProvider>().enabled = true;

            if(movementScript.GetComponentInChildren<SnapTurnProvider>().enabled == true)
            {
                logScript.GetComponent<LogText>().textval += "snap on";
            }
            else
            {
                logScript.GetComponent<LogText>().textval += "nope";
            }
        });
    }
}
