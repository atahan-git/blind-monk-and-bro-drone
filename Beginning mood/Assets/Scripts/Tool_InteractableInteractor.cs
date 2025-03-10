using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;

public class Tool_InteractableInteractor : MonoBehaviour, IPlayerTool {
    
    public LayerMask layerMask;

    public ObjectSelectionHelper<Interactable> selector = new ObjectSelectionHelper<Interactable>();
    public HighlightProfile highlight_interactable;

    public bool Interact(InteractInput interactInput)
    {
        Ray ray = new Ray(interactInput.interactSource.position, interactInput.interactSource.forward);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 20, layerMask)) {
            var gm = hitInfo.collider.gameObject;
            Interactable interactable;
            interactable = gm.GetComponent<Interactable>();
            if (interactable == null) {
                interactable = gm.GetComponentInParent<Interactable>();
            }

            if (interactable != null) {
                selector.Select(interactable, highlight_interactable);
            } else {
                selector.Deselect();
            }
        }else {
           selector.Deselect();
        }

        if (interactInput.interactDown) {
            if (selector.curObject != null) {
                selector.curObject.Interact();
            }
        }

        return selector.curObject != null;
    }

    public bool HoldAttention() {
        return false;
    }


    public void DisableTool() {
        selector.Deselect();
    }
}


