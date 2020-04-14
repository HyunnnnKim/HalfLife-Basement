using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using HalfLight.Movement;

namespace HalfLight.Output
{
    public class LogController : MonoBehaviour
    {
        #region Buttons
        [SerializeField] private Button smoothBtn = null;
        [SerializeField] private Button snapBtn = null;
        #endregion

        #region Components
        [SerializeField] private MovementHandler movementScript = null;
        [SerializeField] private TextMeshPro eventLog = null;
        #endregion

        #region BuiltIn Methods
        private void Awake()
        {

        }

        void Start()
        {
            #region Rotation Buttons
            smoothBtn.onClick.AddListener(() =>
            {
                movementScript.GetComponent<MovementHandler>();
                movementScript.SelectedRotation = MovementHandler.rotateType.SmoothRotation;

                movementScript.GetComponentInChildren<SnapTurnProvider>().enabled = false;

                if (movementScript.GetComponentInChildren<SnapTurnProvider>().enabled == false)
                    eventLog.GetComponent<EventLog>().eventList = "[Command Successful] Snap is off.\n";
                else
                    eventLog.GetComponent<EventLog>().eventList = "[Command Failed] Can not execute.\n";
            });

            snapBtn.onClick.AddListener(() =>
            {
                movementScript.GetComponent<MovementHandler>();
                movementScript.SelectedRotation = MovementHandler.rotateType.Snapturn;

                movementScript.GetComponentInChildren<SnapTurnProvider>().enabled = true;

                if (movementScript.GetComponentInChildren<SnapTurnProvider>().enabled == true)
                    eventLog.GetComponent<EventLog>().eventList = "[Command Successful] Snap is on.\n";
                else
                    eventLog.GetComponent<EventLog>().eventList = "[Command Failed] Can not execute.\n";
            });
            #endregion
        }
        #endregion
    }
}