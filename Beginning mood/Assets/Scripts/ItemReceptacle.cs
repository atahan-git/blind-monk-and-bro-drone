using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;
using UnityEngine.Events;

public class ItemReceptacle : MonoBehaviour {
    public Transform holder;

    public UnityEvent OnPowered = new UnityEvent();
    private void OnTriggerEnter(Collider other) {
        if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<Battery>() != null) {
            other.GetComponent<Carryable>().DestroySelf();
            other.transform.position = holder.transform.position;
            other.transform.rotation = holder.transform.rotation;
            OnPowered?.Invoke();
            
            GetComponentInChildren<AudioPlayer>().PlayOnce();
        }
    }
}
