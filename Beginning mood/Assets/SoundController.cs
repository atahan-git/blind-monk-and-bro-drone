using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {
    public static SoundController s;

    private void Awake() {
        s = this;
    }

    public AudioClip wind;

    public AudioClip lilly;
    private AudioSource _source;


    private void Start() {
        _source = GetComponent<AudioSource>();
    }

    public void DoFlip() {
        StartCoroutine(_DoFlip());
    }

    IEnumerator _DoFlip() {
        var volume = _source.volume;


        while (volume > 0f) {
            volume -= Time.deltaTime;
            _source.volume = volume;
            yield return null;
        }

        yield return new WaitForSeconds(5f);

        _source.Stop();
        _source.clip = lilly;
        _source.volume = 1f;
        _source.loop = false;
        _source.Play();
    }
}
