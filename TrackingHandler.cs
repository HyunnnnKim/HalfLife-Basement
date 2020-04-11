using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using HalfLight.Input;


namespace HalfLight.Movement
{
    public class TrackingHandler : MonoBehaviour
    {
        #region VARIABLES
            #region Serialized Variables
            
            #endregion

            #region Private Variables
                private ControllerInput _controllerInput;
                private Transform _head;
                private Transform _leftHand;
                private Transform _rightHand;
            #endregion
        #endregion

        #region BUILTIN METHODS
            void Start()
            {
                _controllerInput = ControllerInput.Instance;
            }

            void Update()
            {
                // _leftHand.transform = _controllerInput.getLeftController.transform;
            }
        #endregion
    }
}