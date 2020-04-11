using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HalfLight.Rig
{
    public class IgnoreColliding : MonoBehaviour
    {
        #region VARIABLES
            #region Serialized Variables
                [SerializeField] private GameObject head;
                [SerializeField] private GameObject neck;
                [SerializeField] private GameObject chest;
                [SerializeField] private GameObject lShoulder;
                [SerializeField] private GameObject lUpArm;
                [SerializeField] private GameObject lLowerArm;
                [SerializeField] private GameObject lHand;
                [SerializeField] private GameObject lUpThumb;
                [SerializeField] private GameObject lLowerThumb;
                [SerializeField] private GameObject lUpIndex;
                [SerializeField] private GameObject lLowerIndex;
                [SerializeField] private GameObject lUpMiddle;
                [SerializeField] private GameObject lLowerMiddle;
                [SerializeField] private GameObject lUpRing;
                [SerializeField] private GameObject lLowerRing;
                [SerializeField] private GameObject lUpPink;
                [SerializeField] private GameObject lLowerPink;

                [SerializeField] private GameObject rShoulder;
                [SerializeField] private GameObject rUpArm;
                [SerializeField] private GameObject rLowerArm;
                [SerializeField] private GameObject rHand;
                [SerializeField] private GameObject rUpThumb;
                [SerializeField] private GameObject rLowerThumb;
                [SerializeField] private GameObject rUpIndex;
                [SerializeField] private GameObject rLowerIndex;
                [SerializeField] private GameObject rUpMiddle;
                [SerializeField] private GameObject rLowerMiddle;
                [SerializeField] private GameObject rUpRing;
                [SerializeField] private GameObject rLowerRing;
                [SerializeField] private GameObject rUpPink;
                [SerializeField] private GameObject rLowerPink;
                [SerializeField] private GameObject spine;
                [SerializeField] private GameObject hip;
                [SerializeField] private GameObject lUpLeg;
                [SerializeField] private GameObject lLowerLeg;
                [SerializeField] private GameObject lFeet;
                [SerializeField] private GameObject lToe;
                [SerializeField] private GameObject rUpLeg;
                [SerializeField] private GameObject rLowerLeg;
                [SerializeField] private GameObject rFeet;
                [SerializeField] private GameObject rToe;
            #endregion

            #region Private Variables

            #endregion
        #endregion

        #region BUILTIN METHODS
            private void Awake() {
                Physics.IgnoreCollision(head.GetComponent<Collider>(), neck.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(neck.GetComponent<Collider>(), chest.GetComponent<Collider>(), true);

                Physics.IgnoreCollision(neck.GetComponent<Collider>(), lShoulder.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(chest.GetComponent<Collider>(), lUpArm.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lUpArm.GetComponent<Collider>(), lLowerArm.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lLowerArm.GetComponent<Collider>(), lHand.GetComponent<Collider>(), true);

                Physics.IgnoreCollision(lHand.GetComponent<Collider>(), lUpThumb.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lUpThumb.GetComponent<Collider>(), lLowerThumb.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lHand.GetComponent<Collider>(), lUpIndex.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lUpIndex.GetComponent<Collider>(), lLowerIndex.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lHand.GetComponent<Collider>(), lUpMiddle.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lUpMiddle.GetComponent<Collider>(), lLowerMiddle.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lHand.GetComponent<Collider>(), lUpRing.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lUpRing.GetComponent<Collider>(), lLowerRing.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lHand.GetComponent<Collider>(), lUpPink.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lUpPink.GetComponent<Collider>(), lLowerPink.GetComponent<Collider>(), true);

                Physics.IgnoreCollision(neck.GetComponent<Collider>(), rShoulder.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(chest.GetComponent<Collider>(), rUpArm.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rUpArm.GetComponent<Collider>(), rLowerArm.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rLowerArm.GetComponent<Collider>(), rHand.GetComponent<Collider>(), true);

                Physics.IgnoreCollision(rHand.GetComponent<Collider>(), rUpThumb.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rUpThumb.GetComponent<Collider>(), rLowerThumb.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rHand.GetComponent<Collider>(), rUpIndex.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rUpIndex.GetComponent<Collider>(), rLowerIndex.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rHand.GetComponent<Collider>(), rUpMiddle.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rUpMiddle.GetComponent<Collider>(), rLowerMiddle.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rHand.GetComponent<Collider>(), rUpRing.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rUpRing.GetComponent<Collider>(), rLowerRing.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rHand.GetComponent<Collider>(), rUpPink.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rUpPink.GetComponent<Collider>(), rLowerPink.GetComponent<Collider>(), true);

                Physics.IgnoreCollision(chest.GetComponent<Collider>(), spine.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(spine.GetComponent<Collider>(), hip.GetComponent<Collider>(), true);

                Physics.IgnoreCollision(hip.GetComponent<Collider>(), lUpLeg.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(hip.GetComponent<Collider>(), rUpLeg.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lUpLeg.GetComponent<Collider>(), lLowerLeg.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lLowerLeg.GetComponent<Collider>(), lFeet.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(lFeet.GetComponent<Collider>(), lToe.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rUpLeg.GetComponent<Collider>(), rLowerLeg.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rLowerLeg.GetComponent<Collider>(), rFeet.GetComponent<Collider>(), true);
                Physics.IgnoreCollision(rFeet.GetComponent<Collider>(), rToe.GetComponent<Collider>(), true);
            }
        #endregion
    }
}
