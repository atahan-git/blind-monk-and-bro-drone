using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PoweredDoor : MonoBehaviour
{
    public bool isOpen;

    public GameObject door;

    private Vector3 closePos;

    public TMP_Text text;

    public UnityEvent OnDoorFullyOpen = new UnityEvent();
    private bool invoked = false;
    private AudioPlayer _audioPlayer;
    void Start() {
        _audioPlayer = GetComponentInChildren<AudioPlayer>();
        closePos = door.transform.localPosition;
        Invoke(nameof(Close), 1f);
    }

    // Update is called once per frame
    void Update() {
        var targetPos = isOpen ? closePos + Vector3.up * 7.2f : closePos;
        var moveSpeed = isOpen ? 10 : 10;
        
        door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, targetPos, moveSpeed*Time.deltaTime);

        var isStopped = Vector3.Distance(door.transform.localPosition, targetPos) < 0.1f;

        if (isStopped) {
            _audioPlayer.Stop();
        } else {
            _audioPlayer.Play();
        }

        if (!invoked && isOpen && isStopped) {
            OnDoorFullyOpen?.Invoke();
        }
    }

    public void Open() {
        isOpen = true;
        text.text = "yes power";
    }

    public void Close(){
        isOpen = false;
        text.text = "No Power";
    }
}
