using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;

public class Tool_HandGrab : MonoBehaviour, IPlayerTool
{
    

    public ObjectSelectionHelper<Rigidbody> selector = new ObjectSelectionHelper<Rigidbody>();
    public LayerMask layerMask;

    public HighlightProfile highlightProfile;
    public HighlightProfile selectProfile;

    public bool isHolding = false;
    public float holdDistance = 2;

    private InteractInput lastInput;

    public float range = 4;

    public bool Interact(InteractInput interactInput)
    {
        if (selector.curObject == null || selector.curObject.gameObject == null || !selector.curObject.GetComponent<Carryable>()) {
            StopHolding();
            selector.curObject = null;
        }

        bool grabAttention = false;
        if (!isHolding) {
            Ray ray = new Ray(interactInput.interactSource.position, interactInput.interactSource.forward);

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, range, layerMask)) {

                Carryable carryable = null;
                if (hitInfo.rigidbody != null) {
                    carryable = hitInfo.rigidbody.GetComponent<Carryable>();
                }
                
                if (carryable) {
                    selector.Select(hitInfo.rigidbody, highlightProfile);

                    if (interactInput.interactDown) {
                        selector.Select(selector.curObject, selectProfile);
                        StartHolding();
                        grabAttention = true;
                    }
                } else {
                    selector.Deselect();
                }
            } else {
                selector.Deselect();
            }
        } else {
            if (interactInput.interactDown) {
                StopHolding();
                grabAttention = true;
            }
        }

        lastInput = interactInput;

        return selector.curObject != null || grabAttention;
    }

    public bool HoldAttention() {
        return false;
    }

    void StopHolding() {
        isHolding = false;
        if (selector.curObject != null && !selector.curObject.GetComponent<Carryable>()) {
            selector.Deselect();
        }
    }


    void StartHolding() {
        if (selector.curObject != null) {
            isHolding = true;
        }
    }

    public void DisableTool() {
    }

    private void Update() {
        if (isHolding) {
            if (selector.curObject == null || selector.curObject.gameObject == null || !selector.curObject.GetComponent<Carryable>()) {
                StopHolding();
                return;
            }

            var targetPos = lastInput.interactSource.position + lastInput.interactSource.transform.forward * holdDistance + Vector3.down*0.4f;

            selector.curObject.transform.position = targetPos;
            selector.curObject.transform.rotation = lastInput.interactSource.rotation;
        }
    }
}
