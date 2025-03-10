using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChanger : MonoBehaviour
{
    
    public float gravity =9.81f;
    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = Vector3.down*gravity;
    }
}
