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
        #region VARIABLES
            #region Buttons
                [SerializeField] private Button smoothBtn = null;
                [SerializeField] private Button snapBtn = null;
                [SerializeField] private Button rbBtn = null;
                [SerializeField] private Button ccBtn = null;
            #endregion
            
            #region Components
                [SerializeField] private MovementHandler movementScript = null; 
                [SerializeField] private TextMeshPro eventLog = null;
            #endregion
        #endregion

        #region BUILTIN METHODS
            private void Awake() {
            
            }

            void Start()
            {
                #region Rotation Buttons
                    smoothBtn.onClick.AddListener(() =>
                    {
                        movementScript.GetComponent<MovementHandler>();
                        movementScript.SelectedRotation = MovementHandler.rotateType.SmoothRotation;

                        movementScript.GetComponentInChildren<SnapTurnProvider>().enabled = false;
                        
                        if(movementScript.GetComponentInChildren<SnapTurnProvider>().enabled == false)
                            eventLog.GetComponent<EventLog>().eventList = "[Command Successful] Snap is off.\n";
                        else
                            eventLog.GetComponent<EventLog>().eventList = "[Command Failed] Can not execute.\n";
                    });

                    snapBtn.onClick.AddListener(() =>
                    {
                        movementScript.GetComponent<MovementHandler>();
                        movementScript.SelectedRotation = MovementHandler.rotateType.Snapturn;

                        movementScript.GetComponentInChildren<SnapTurnProvider>().enabled = true;

                        if(movementScript.GetComponentInChildren<SnapTurnProvider>().enabled == true)
                            eventLog.GetComponent<EventLog>().eventList = "[Command Successful] Snap is on.\n";
                        else
                            eventLog.GetComponent<EventLog>().eventList = "[Command Failed] Can not execute.\n";
                    });
                #endregion

                #region PlayerBody Buttons
                    rbBtn.onClick.AddListener(() =>
                    {
                        movementScript.GetComponent<MovementHandler>();
                        movementScript.SelectedMovement = MovementHandler.movementType.RigidBody;

                        movementScript.GetComponentInChildren<Rigidbody>().isKinematic = false;
                        movementScript.GetComponentInChildren<CharacterController>().enabled = false;

                        if(movementScript.GetComponentInChildren<Rigidbody>().isKinematic == false)
                            eventLog.GetComponent<EventLog>().eventList = "[Command Successful] RB is selected.\n";
                        else
                            eventLog.GetComponent<EventLog>().eventList = "[Command Failed] Can not execute.\n";
                    });

                    ccBtn.onClick.AddListener(() =>
                    {
                        movementScript.GetComponent<MovementHandler>();
                        movementScript.SelectedMovement = MovementHandler.movementType.CharacterController;

                        movementScript.GetComponentInChildren<CharacterController>().enabled = true;
                        movementScript.GetComponentInChildren<CharacterController>().enabled = true;
                        
                        if(movementScript.GetComponentInChildren<CharacterController>().enabled == true)
                            eventLog.GetComponent<EventLog>().eventList = "[Command Successful] CC is selected.\n";
                        else
                            eventLog.GetComponent<EventLog>().eventList = "[Command Failed] Can not execute.\n";
                    });
                #endregion
            }
        #endregion
    }
}