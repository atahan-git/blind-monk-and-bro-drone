using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTool : MonoBehaviour, IPlayerTool {


    public GameObject bullet;
    //public Transform barrel;

    private void Awake() {
        bullet.SetActive(false);
    }

    public bool Interact(InteractInput interactInput) {
        if (interactInput.primaryDown) {
            Instantiate(bullet, bullet.transform.position, bullet.transform.rotation).SetActive(true);
            
            GetComponentInChildren<AudioPlayer>().PlayOnce();
            
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
