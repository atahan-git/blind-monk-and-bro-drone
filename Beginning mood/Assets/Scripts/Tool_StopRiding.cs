using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_StopRiding : MonoBehaviour, IPlayerTool {
    public RideableBody myBody;

    public bool Interact(InteractInput interactInput) {
        if (interactInput.escape) {
            var newBody = myBody.StopRiding();
            interactInput.actingController.AssumeBody(newBody);
            return true;
        }

        return false;
    }

    public bool HoldAttention() {
        return false;
    }

    public void DisableTool() {
        // do nothing?
    }
}
