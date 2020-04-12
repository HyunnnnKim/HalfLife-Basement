using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HalfLight.Movement;

namespace HalfLight.Output
{
    public class StatusLog : MonoBehaviour
    {
        #region VARIABLES
            #region Serialized Variables
                [SerializeField] private MovementHandler playerStat;
            #endregion

            #region Private Variables
                private TextMeshPro statusLog;
                private string _statusLogText;
            #endregion
        #endregion

        #region BUILTIN METHODS
            void Start()
            {
                statusLog = GetComponent<TextMeshPro>();
                playerStat.GetComponent<MovementHandler>();
            }

            void Update()
            {
                _statusLogText = "STATUS\n";
                _statusLogText += "--------------------------------\n";
                _statusLogText += "Rotation\t: " + playerStat.SelectedRotation + "\n";
                _statusLogText += "Speed\t: " + playerStat.CurrentSpeed.ToString() + "\n";
                _statusLogText += "Distance\t: " + playerStat.Distance.ToString() + "\n";

                statusLog.text = _statusLogText;
            }
        #endregion
    }
}