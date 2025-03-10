using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;

public class Tool_BodySwapper : MonoBehaviour, IPlayerTool {
    
    public LayerMask layerMask;

    public ObjectSelectionHelper<ControllableBody> selector = new ObjectSelectionHelper<ControllableBody>();
    public HighlightProfile highlight_interactable;

    public bool Interact(InteractInput interactInput)
    {
        Ray ray = new Ray(interactInput.interactSource.position, interactInput.interactSource.forward);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 20, layerMask)) {
            var gm = hitInfo.collider.gameObject;
            ControllableBody controllableBody;
            controllableBody = gm.GetComponent<ControllableBody>();
            if (controllableBody == null) {
                controllableBody = gm.GetComponentInParent<ControllableBody>();
            }

            if (controllableBody != null && controllableBody.body != interactInput.actingController.currentBody) {
                selector.Select(controllableBody, highlight_interactable);
            } else {
                selector.Deselect();
            }
        }else {
            selector.Deselect();
        }

        bool interacted = false;
        if (interactInput.interactDown) {
            if (selector.curObject != null) {
                if (selector.curObject is RideableBody rideableBody) {
                    rideableBody.SetRidingBody(interactInput.actingController.currentBody);
                }

                if (selector.curObject is EscapeableBody escapeableBody) {
                    escapeableBody.SetEscapeableBody(interactInput.actingController.currentBody);
                }
                
                interactInput.actingController.AssumeBody(selector.curObject.body);
                interacted = true;
            }
        }

        return selector.curObject != null || interacted;
    }

    public bool HoldAttention() {
        return false;
    }


    public void DisableTool() {
        selector.Deselect();
    }
}