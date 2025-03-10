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
            var copy = Instantiate(other.attachedRigidbody.gameObject, holder.transform.position, holder.transform.rotation, holder);
            Destroy(copy.GetComponent<Rigidbody>());
            Destroy(other.gameObject);
            Destroy(copy.GetComponent<HighlightEffect>());
            Destroy(copy.GetComponent<Battery>());
            Destroy(copy.GetComponent<Carryable>());
            Destroy(copy.GetComponent<HitSoundSource>());
            OnPowered?.Invoke();
            
            GetComponentInChildren<AudioPlayer>().PlayOnce();
        }
    }
}
