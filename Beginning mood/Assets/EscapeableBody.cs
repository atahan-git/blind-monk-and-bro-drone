using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeableBody : ControllableBody
{
    public ControllableBodyStruct previousBody;
    public bool holdBody = false;
    public Transform bodyHoldZone;
    public bool transportBodyOnEscape = false;
    public Transform bodyEscapeZone;

    private HolderRigidbody _holderRigidbody;

    public void SetEscapeableBody(ControllableBodyStruct bodyStruct) {
        previousBody = bodyStruct;
        if (holdBody) {
            _holderRigidbody = new HolderRigidbody(previousBody.body.GetComponent<Rigidbody>());
            Destroy(previousBody.body.GetComponent<Rigidbody>());
            
            bodyStruct.body.transform.SetParent(bodyHoldZone);
            bodyStruct.body.transform.localPosition = Vector3.zero;
            bodyStruct.body.transform.localRotation = Quaternion.identity;
        }
    }

    public ControllableBodyStruct EscapingIntoBody() {
        if (holdBody) {
            previousBody.body.transform.SetParent(null);
            
            var rg = previousBody.body.AddComponent<Rigidbody>();
            _holderRigidbody.Apply(rg);
        }

        if (transportBodyOnEscape) {
            previousBody.body.transform.position = bodyEscapeZone.position;
            previousBody.body.transform.rotation = bodyEscapeZone.rotation;
        }

        var toEscape = previousBody;
        previousBody = null;
        return toEscape;
    }
}
