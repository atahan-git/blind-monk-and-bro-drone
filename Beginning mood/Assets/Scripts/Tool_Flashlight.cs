using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_Flashlight : MonoBehaviour, IPlayerTool {
    public bool isOn = false;
    void Start()
    {
        ApplyLampState();
    }

    public void ApplyLampState() {
        GetComponentInChildren<Light>().enabled = isOn;
    }

    public bool Interact(InteractInput interactInput) {
        if (interactInput.lightKey) {
            isOn = !isOn;
            ApplyLampState();
            return true;
        }

        return false;
    }

    public bool HoldAttention() {
        return false;
    }

    public void DisableTool() {
    }
}
