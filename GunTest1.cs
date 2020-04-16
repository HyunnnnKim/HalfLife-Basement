using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRcustom;

public class GunTest1 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bullet;
    
    [SerializeField]
    Transform bulletTransform;
    PlayerInput playerInput;
    
    void Start()
    {
        playerInput = PlayerInput.Instance;
    }

    private void Update()
    {
        if (playerInput.IsButtonPushed(ButtonName.trigger, VRDviceNode.RightHand)
            && playerInput.GetHandInputData(VRDviceNode.RightHand).commonButtonStatus.gripButton)
        {
            Instantiate(bullet, bulletTransform.position, bulletTransform.rotation);
        }
    }
}
