using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Start() {
        GetComponent<Rigidbody>().velocity = transform.forward * 40;
    }

}
