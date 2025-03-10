using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonContactingCable : PowerCable {

    public Transform contactPoint;

    public Transform contactTarget;

    public override bool IsWorking() {
        return Vector3.Distance(contactPoint.position, contactTarget.position) < 0.1f;
    }
}
