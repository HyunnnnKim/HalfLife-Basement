using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using HalfLife.Movement;

namespace HalfLife.Output
{
    public class LogInterface : MonoBehaviour
    {
        #region Serialied Variables
            [SerializeField] private Button smoothBtn = null;
            [SerializeField] private Button snapBtn = null;

            [SerializeField] private GameObject cube = null;
            [SerializeField] private CharacterControllerMovement movementScript = null;
            [SerializeField] private TextMeshPro eventLog = null;
        #endregion

        #region BuiltIn Methods
            private void Awake() {
            
            }

            void Start()
            {
                smoothBtn.onClick.AddListener(() =>
                {
                    cube.gameObject.GetComponent<Rigidbody>().AddForce(3000f * Vector3.up);

                    movementScript.GetComponent<CharacterControllerMovement>();
                    movementScript.SelectedRotation = CharacterControllerMovement.rotateType.SmoothRotation;

                    movementScript.GetComponentInChildren<SnapTurnProvider>().enabled = false;

                    if(movementScript.GetComponentInChildren<SnapTurnProvider>().enabled == false)
                    {
                        eventLog.GetComponent<EventLog>().eventList = "snap off\n";
                    }
                    else
                    {
                        eventLog.GetComponent<EventLog>().eventList = "nope\n";
                    }
                });

                snapBtn.onClick.AddListener(() =>
                {
                    cube.gameObject.GetComponent<Rigidbody>().AddForce(3000f * Vector3.right);

                    movementScript.GetComponent<CharacterControllerMovement>();
                    movementScript.SelectedRotation = CharacterControllerMovement.rotateType.Snapturn;

                    movementScript.GetComponentInChildren<SnapTurnProvider>().enabled = true;

                    if(movementScript.GetComponentInChildren<SnapTurnProvider>().enabled == true)
                    {
                        eventLog.GetComponent<EventLog>().eventList = "snap on\n";
                    }
                    else
                    {
                        eventLog.GetComponent<EventLog>().eventList = "nope\n";
                    }
                });
            }
        #endregion
    }
}