using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairableCable : PowerCable {
    public GameObject burn;

    public override bool IsWorking() {
        return burn == null;
    }
}
