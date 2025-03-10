using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeatSensor : MonoBehaviour {

    public int detectionTemp = 4;
    
    public UnityEvent OnPowered = new UnityEvent();
    public UnityEvent OnLosePower = new UnityEvent();

    private bool curPowerState = false;
    void Update() {
        var detectCorrect = false;

        for (int i = 0; i < objectsInside.Count; i++) {
            if (objectsInside[i].temperature >= detectionTemp) {
                detectCorrect = true;
            }
        }

        if (curPowerState != detectCorrect) {
            curPowerState = detectCorrect;
            if (curPowerState) {
                OnPowered?.Invoke();
            } else {
                OnLosePower?.Invoke();
            }
        }
    }

    public List<Heatable> objectsInside = new List<Heatable>();
    private void OnTriggerEnter(Collider other) {
        //print("enter");
        if (other.attachedRigidbody != null) {
            var heatable = other.attachedRigidbody.GetComponent<Heatable>();

            if (heatable != null) {
                if (!objectsInside.Contains(heatable)) {
                    objectsInside.Add(heatable);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        //print("exit");
        if (other.attachedRigidbody != null) {
            var heatable = other.attachedRigidbody.GetComponent<Heatable>();

            if (heatable != null) {
                objectsInside.Remove(heatable);
            }
        }
    }
}
