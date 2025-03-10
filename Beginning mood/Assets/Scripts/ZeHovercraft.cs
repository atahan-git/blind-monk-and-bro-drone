using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeHovercraft : MonoBehaviour
{
    public Transform camPos;
    public Transform camera;
    void Start()
    {
        camera.SetParent(null);

        for (int i = 0; i < camera.childCount; i++) {
            camera.GetChild(i).gameObject.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate() {
        camera.position = Vector3.Lerp(camera.position, camPos.transform.position, 20 * Time.deltaTime);
        var lookDir = camPos.forward;
        var lookRot = Quaternion.LookRotation(lookDir, Vector3.up);
        camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, lookRot, 50 * Time.deltaTime);
    }
}
