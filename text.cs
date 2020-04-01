using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HalfLife.Input;
using HalfLife.Movement;

public class text : MonoBehaviour
{
    private TextMeshPro tmp;
    private PlayerInput pi;

    void Start()
    {
        pi = PlayerInput.Instance;
        tmp = GetComponent<TextMeshPro>();
        tmp.text = "hi";
    }


    void Update()
    {
        //tmp.text = pi.GetInput().ToString();
        tmp.text = pi.get

    }
}
