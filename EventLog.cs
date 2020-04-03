using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace HalfLight.Output
{
    public class EventLog : MonoBehaviour
    {
        #region Private Variables
            private TextMeshPro eventLog;
            private string _eventLogText;
            private List<string> events = new List<string>();
            public string eventList { get { return events[0]; } set { if(events.Count > 7) events.RemoveAt(0); events.Add(value); } }
        #endregion

        #region BuiltIn Methods
            void Start()
            {
                eventLog = GetComponent<TextMeshPro>();
            }

            void Update()
            {
                _eventLogText = "EVENT\n";
                _eventLogText += "--------------------\n";
                
                foreach (string e in events)
                {
                    _eventLogText += e;
                }

                eventLog.text = _eventLogText;
            }
        #endregion
    }    
}