using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSoundSource : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision) {
        HitSoundsController.s.MakeHitSound(collision);
    }

}
