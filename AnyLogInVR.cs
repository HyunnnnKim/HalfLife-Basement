using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnyLogInVR : MonoBehaviour
{
    TextMeshPro textMeshPro;
    string text;
    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        
    }

    // Update is called once per frame
    void Update()
    {
        textMeshPro.text = text;
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        text += "\n" + logString;

    }
}
