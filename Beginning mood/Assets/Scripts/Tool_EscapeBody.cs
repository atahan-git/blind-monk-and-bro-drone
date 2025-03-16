using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_EscapeBody : MonoBehaviour, IPlayerTool {
    public EscapeableBody myBody;

    public bool Interact(InteractInput interactInput) {
        if (interactInput.droneKey) {
            var newBody = myBody.EscapingIntoBody();
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
