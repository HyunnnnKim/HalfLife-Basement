using System;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.XR;

namespace VRcustom
{
    /// <summary>
    /// XRController MonoBehaviour that interprets InputSystem events into 
    /// XR Interaction Interactor position, rotation and interaction states.
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("Custom/VR Controller Tracker")]
    public class VRControllerTracker : MonoBehaviour
    {
        PlayerInput playerInput;
        /// <summary>
        /// The update type being used by the tracked pose driver
        /// </summary>
        public enum UpdateType
        {
            /// <summary>
            /// Sample input at both update, and directly before rendering. For smooth head pose tracking, 
            /// we recommend using this value as it will provide the lowest input latency for the device. 
            /// This is the default value for the UpdateType option
            /// </summary>
            UpdateAndBeforeRender,
            /// <summary>
            /// Only sample input during the update phase of the frame.
            /// </summary>
            Update,
            /// <summary>
            /// Only sample input directly before rendering
            /// </summary>
            BeforeRender,
        }
        [Header("Tracking Configuration")]

        [SerializeField]
        [Tooltip("The time within the frame that the XRController will sample input.")]
        UpdateType m_UpdateTrackingType = UpdateType.UpdateAndBeforeRender;
        /// <summary>
        /// The update type being used by the tracked pose driver
        /// </summary>
        public UpdateType updateTrackingType
        {
            get { return m_UpdateTrackingType; }
            set { m_UpdateTrackingType = value; }
        }

        bool m_EnableInputTracking = true;
        /// <summary>Gets or sets if input is enabled for this controller.</summary>
        public bool enableInputTracking
        {
            get { return m_EnableInputTracking; }
            set { m_EnableInputTracking = value; }
        }


        [Header("Configuration")]

        [SerializeField]
        [Tooltip("Gets or sets the XRNode for this controller.")]
        VRDviceNode m_ControllerNode;

        /// <summary>Gets the InputDevice being used to read data from.</summary>
        public InputDevice inputDevice
        {
            get
            {
                return m_InputDevice.isValid ? m_InputDevice : (m_InputDevice = playerInput.GetVRDevice(m_ControllerNode));
            }
        }
        private InputDevice m_InputDevice;

        protected virtual void OnEnable()
        {
            Application.onBeforeRender += OnBeforeRender;
        }

        protected virtual void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
        }

        protected virtual void OnBeforeRender()
        {
            if (enableInputTracking &&
               (m_UpdateTrackingType == UpdateType.BeforeRender ||
                m_UpdateTrackingType == UpdateType.UpdateAndBeforeRender))
            {
                UpdateTrackingInput();
            }
        }


        private void Start()
        {
            playerInput = PlayerInput.Instance;
        }

        void Update()
        {

            if (enableInputTracking &&
                (m_UpdateTrackingType == UpdateType.Update ||
                m_UpdateTrackingType == UpdateType.UpdateAndBeforeRender))
            {
                UpdateTrackingInput();

            }

        }

        protected void UpdateTrackingInput()
        {
            Vector3 devicePosition = new Vector3();
            Quaternion deviceRotation = new Quaternion();
            if (inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition))
                transform.localPosition = devicePosition;

            if (inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out deviceRotation))
                transform.localRotation = deviceRotation;
        }
    }
}
