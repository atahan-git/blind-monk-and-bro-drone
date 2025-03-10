using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DoorOpener : MonoBehaviour {

    public bool isOpen;

    public GameObject door;

    private Vector3 closePos;

    public TMP_Text text;
    void Start() {
        closePos = door.transform.localPosition;
        Invoke(nameof(Close), 1f);
    }

    void Close() {
        isOpen = false;
        text.text = "No Power";
    }

    // Update is called once per frame
    void Update() {
        var targetPos = isOpen ? closePos + Vector3.up * 7.2f : closePos;
        
        door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, targetPos, 1*Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        isOpen = true;
        text.text = "yes power";
    }

    private void OnTriggerExit(Collider other) {
        isOpen = false;
        text.text = "No Power";
    }
}
