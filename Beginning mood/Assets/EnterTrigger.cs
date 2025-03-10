using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnterTrigger : MonoBehaviour
{
    [Header("only works for train for now")]
    public UnityEvent OnEnter = new UnityEvent();
    private void OnTriggerEnter(Collider other) {
        if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<Train>() != null) {
            
            OnEnter?.Invoke();
        }
    }
}
