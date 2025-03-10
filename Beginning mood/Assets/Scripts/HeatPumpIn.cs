using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatPumpIn : MonoBehaviour {
    private HeatPump myPump;

    private void Start() {
        myPump = GetComponentInParent<HeatPump>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.attachedRigidbody != null) {
            var heatable = other.attachedRigidbody.GetComponent<Heatable>();

            if (heatable != null) {
                if (!myPump.heatPumpIn.Contains(heatable)) {
                    myPump.heatPumpIn.Add(heatable);
                    myPump.NumbersChanged();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.attachedRigidbody != null) {
            var heatable = other.attachedRigidbody.GetComponent<Heatable>();

            if (heatable != null) {
                myPump.heatPumpIn.Remove(heatable);
                myPump.NumbersChanged();
            }
        }
    }
}
