using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideableBody : ControllableBody
{
    public ControllableBodyStruct ridingBody;
    public Transform bodyHoldZone;
    public Transform bodyEscapeZone;

    private HolderRigidbody _holderRigidbody;

    private bool isRiding = false;
    public void SetRidingBody(ControllableBodyStruct bodyStruct) {
        isRiding = true;
        body.cam = bodyStruct.cam;
        ridingBody = bodyStruct;

        _holderRigidbody = new HolderRigidbody(ridingBody.body.GetComponent<Rigidbody>());
        Destroy(ridingBody.body.GetComponent<Rigidbody>());
        
        ridingBody.body.transform.SetParent(bodyHoldZone);
        ridingBody.body.transform.localPosition = Vector3.zero;
        ridingBody.body.transform.localRotation = Quaternion.identity;
    }

    public ControllableBodyStruct StopRiding() {
        isRiding = false;
        ridingBody.body.transform.SetParent(null);
        ridingBody.body.transform.position = bodyEscapeZone.position;
        ridingBody.body.transform.rotation = bodyEscapeZone.rotation;

        var rg = ridingBody.body.AddComponent<Rigidbody>();
        _holderRigidbody.Apply(rg);
        
        var oldBody = ridingBody;
        ridingBody = null;

        body.cam = null;

        return oldBody;
    }

    private void Update() {
        if (isRiding) {
            ridingBody.body.transform.localPosition = Vector3.zero;
            ridingBody.body.transform.localRotation = Quaternion.identity;
        }
    }
}
