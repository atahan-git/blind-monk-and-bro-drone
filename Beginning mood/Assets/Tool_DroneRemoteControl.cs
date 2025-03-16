using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_DroneRemoteControl : MonoBehaviour,IPlayerTool {

    public SmartDrone drone;
    public LookCamera lookCamera;
    public ExternalCamera externalCamera;
    public LayerMask layerMask;
    
    public bool Interact(InteractInput interactInput) {
        if (interactInput.toggleDroneFollow) {
            drone.ToggleFollowPlayerMode();
            return true;
        }

        if (interactInput.toggleDroneVision) {
            externalCamera.doRenderTexture = !externalCamera.doRenderTexture;
        }

        if (interactInput.setDroneLookTarget) {
            Ray ray = new Ray(interactInput.interactSource.position, interactInput.interactSource.forward);

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000, layerMask)) {
                lookCamera.transform.rotation = Quaternion.LookRotation(hitInfo.point-lookCamera.transform.position);
            }
        }

        return false;
    }

    public bool HoldAttention() {
        return false;
    }

    public void DisableTool() {
        
    }
}
