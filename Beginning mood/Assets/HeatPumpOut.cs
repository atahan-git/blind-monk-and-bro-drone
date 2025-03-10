using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatPumpOut : MonoBehaviour
{
    private HeatPump myPump;

    private void Start() {
        myPump = GetComponentInParent<HeatPump>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.attachedRigidbody != null) {
            var heatable = other.attachedRigidbody.GetComponent<Heatable>();

            if (heatable != null) {
                if (!myPump.heatPumpIn.Contains(heatable)) {
                    myPump.heatPumpOut.Add(heatable);
                    myPump.NumbersChanged();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.attachedRigidbody != null) {
            var heatable = other.attachedRigidbody.GetComponent<Heatable>();

            if (heatable != null) {
                myPump.heatPumpOut.Remove(heatable);
                myPump.NumbersChanged();
            }
        }
    }
}
