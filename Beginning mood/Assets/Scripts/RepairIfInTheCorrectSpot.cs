using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;

public class RepairIfInTheCorrectSpot : MonoBehaviour {

    public GameObject burnEffect;
    public Transform targetPos;

    public Transform fixedTransform;

    public TrainPathPoint toFix;
    
    void Update()
    {
        if (burnEffect == null) {
            Destroy(GetComponent<HingeJoint>());
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<HighlightEffect>());
            transform.position = fixedTransform.position;
            transform.rotation = fixedTransform.rotation;
            Destroy(this);
            toFix.canMoveHere = true;
            return;
        }
        
        
        if (Vector3.Distance(targetPos.position, burnEffect.transform.position) < 0.1f) {
            burnEffect.SetActive(true);
        } else {
            burnEffect.SetActive(false);
        }
    }
}
