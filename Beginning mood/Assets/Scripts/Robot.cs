using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour {


    public bool moveForward = true;

    void FixedUpdate() {
        if (moveForward) {
            GetComponent<Rigidbody>().AddForce(transform.forward * 400 * Time.deltaTime);
        } 
    }
}
