using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSoundsController : MonoBehaviour {
    public static HitSoundsController s;

    public GameObject hitSound;
    private void Awake() {
        s = this;
    }

    public void MakeHitSound(Collision collision) {
	    var sound = Instantiate(hitSound, collision.GetContact(0).point, Quaternion.identity, transform);
        sound.SetActive(true);
        //print(collision.impulse.magnitude);
        sound.GetComponent<AudioPlayer>().PlayOnce(Mathf.Min(collision.impulse.magnitude/5f,4f));
        Destroy(sound, 1f);
    }
}
