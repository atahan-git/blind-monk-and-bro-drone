using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicOnTriggerEnterFirstTime : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
