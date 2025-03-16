using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychDoorLight : MonoBehaviour
{
    public static List<PsychDoorLight> cables = new List<PsychDoorLight>();

    public int cableId = -1;

    public bool isOn;
    private void Awake() {
        cables.Add(this);
    }

    private void OnDestroy() {
        cables.Remove(this);
    }

    public void ApplyState() {
        if (isOn) {
            GetComponentInChildren<Light>().color = Color.green;
        }
    }
}
