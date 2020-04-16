using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTest1 : MonoBehaviour
{
    [SerializeField]
    float bulletSpeed = 1f;
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(-transform.forward * bulletSpeed, ForceMode.Impulse);
        Destroy(gameObject, 5);
    }
}
