using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HalfLight.Rig
{
    [System.Serializable]
    public class VRMap
    {
        public Transform vrTarget;
        public Transform rigTarget;
        public Vector3 trackingPositionOffset;
        public Vector3 trackingRotationOffset;

        public void Map()
        {
            rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
            rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        }
    }

    public class VRRig : MonoBehaviour
    {
        #region Variables
            public VRMap head;
            public VRMap leftHand;
            public VRMap rightHand;

            public Transform headConstraint;
            public Vector3 headBodyOffest;
            public float turnSmooth;
        #endregion

        #region BuiltIn Methods
            private void Start() {
                headBodyOffest = transform.position - headConstraint.position;
            }

            private void Update() {
                transform.position = headConstraint.position + headBodyOffest;

                /* This will rotate the player body according to the head rotation. */
                transform.forward = Vector3.Lerp(transform.forward,
                        Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized, Time.deltaTime * turnSmooth);

                head.Map();
                leftHand.Map();
                rightHand.Map();
            }
        #endregion
    }
}

