using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_EscapeableBodyOnKey : MonoBehaviour, IPlayerTool
{

    public bool Interact(InteractInput interactInput) {
        if (interactInput.droneKey) {
            var escapeableBody = GetComponent<EscapeableBody>();
            escapeableBody.SetEscapeableBody(interactInput.actingController.currentBody);

            interactInput.actingController.AssumeBody(escapeableBody.body);

            return true;
        }

        return false;
    }

    public bool HoldAttention() {
        // nothing
        return false;
    }

    public void DisableTool() {
        // nothing
    }
}
