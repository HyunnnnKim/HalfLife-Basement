using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HalfLife.Input;
using HalfLife.Movement;

public class text : MonoBehaviour
{
    private TextMeshPro tmp;
    private ControllerInput ci;
    private string textval;


    void Start()
    {
        ci = ControllerInput.Instance;
        tmp = GetComponent<TextMeshPro>();
        tmp.text = "hi";
    }


    void Update()
    {
        textval = ci.getRightHand.primary2DValue + "----\n";
        textval += ci.getLeftHand.primary2DValue + "----\n";
        textval += ci.getRightHand.primaryButtonPressed + "----\n";
        tmp.text = textval;
    }
}