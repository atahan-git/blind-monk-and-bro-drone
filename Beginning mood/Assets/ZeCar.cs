using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeCar : MonoBehaviour {

    public GameObject frontWheel;
    //public HingeJoint hingeJoint;
    public GameObject backWheel;

    public Transform camPos;
    public Transform camera;

    void Start() {
        camera.SetParent(null);

        for (int i = 0; i < camera.childCount; i++) {
            camera.GetChild(i).gameObject.SetActive(false);
        }
        
        GetComponent<Rigidbody>().centerOfMass = Vector3.down*2;
    }

    // Update is called once per frame
    void Update() {
        camera.position = Vector3.Lerp(camera.position, camPos.transform.position, 20 * Time.deltaTime);
        var lookDir = camPos.forward;
        var lookRot = Quaternion.LookRotation(lookDir, Vector3.up);
        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, lookRot, 50 * Time.deltaTime);

        var InpHor = Input.GetAxis("Horizontal");
        var InpVer = Input.GetAxis("Vertical");

        frontWheel.GetComponent<Rigidbody>().AddTorque(frontWheel.transform.up * 1000 * Time.deltaTime * -InpVer);
        //frontWheel.GetComponent<Rigidbody>().AddTorque(Vector3.up*1000*Time.deltaTime*InpHor);

        if (Mathf.Abs(InpHor) > 0.1f) {
            var angularVel = frontWheel.GetComponent<Rigidbody>().angularVelocity;
            angularVel.y = InpHor * 8;
            frontWheel.GetComponent<Rigidbody>().angularVelocity = angularVel;
        }
    }
}
