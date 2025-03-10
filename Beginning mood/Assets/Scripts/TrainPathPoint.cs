using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPathPoint : MonoBehaviour {
    public bool canMoveHere = true;
    public float speedMultiplier = 1f;
    public void Block() {
        canMoveHere = false;
    }

    public void UnBlock() {
        canMoveHere = true;
    }
}
