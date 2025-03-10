using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;

public class Tool_HeatHammer : MonoBehaviour, IPlayerTool {

    public HighlightProfile effectHighlight;
    private ObjectSelectionHelper<Heatable> selector = new ObjectSelectionHelper<Heatable>();
    
    public int heatAmount = 1;

    public float range = 2f;
    public LayerMask layerMask;
    public bool Interact(InteractInput interactInput) {
        Ray ray = new Ray(interactInput.interactSource.position, interactInput.interactSource.forward);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, range, layerMask)) {

            Heatable heatable = null;
            if (hitInfo.rigidbody != null) {
                heatable = hitInfo.rigidbody.GetComponent<Heatable>();
            }
                
            if (heatable) {
                selector.Select(heatable, effectHighlight);

                if (interactInput.primaryDown ) {
                    heatable.temperature += heatAmount;
                    GetComponentInChildren<AudioPlayer>().PlayOnce();
                    return true;
                }
            } else {
                selector.Deselect();
            }
        } else {
            selector.Deselect();
        }
        

        return false;
    }

    public bool HoldAttention() {
        return false;
    }

    public void DisableTool() {
        selector.Deselect();
    }
}