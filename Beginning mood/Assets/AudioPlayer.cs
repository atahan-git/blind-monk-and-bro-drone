using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {
    
    
    public AudioClip[] clips;
    private AudioSource _source;

    public bool randomPitch = true;
    private bool isPlaying = false;
    void Start() {
        _source = GetComponent<AudioSource>();
        _source.clip = clips[0];
        _source.loop = true;
    }

    public void PlayOnce() {
        _source = GetComponent<AudioSource>();
        if (randomPitch) {
            _source.pitch = Random.Range(0.8f, 1.2f);
        }
        _source.PlayOneShot(clips[Random.Range(0,clips.Length)]);
    }
    
    public void PlayOnce(float volume) {
        _source = GetComponent<AudioSource>();
        if (randomPitch) {
            _source.pitch = Random.Range(0.8f, 1.2f);
        }
        _source.PlayOneShot(clips[Random.Range(0,clips.Length)], volume);
    }

    public void Play() {
        _source = GetComponent<AudioSource>();
        if (!isPlaying) {
            isPlaying = true;
            _source.Play();
        }
    }

    public void Stop() {
        _source = GetComponent<AudioSource>();
        if (isPlaying) {
            isPlaying = false;
            _source.Stop();
        }
    }

    public void SetPitch(float pitch) {
        _source.pitch = pitch;
    }
}
