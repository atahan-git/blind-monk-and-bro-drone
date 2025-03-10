using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsPush : MonoBehaviour
{
    public float pushPower = 2.0f;
    public float weight = 6.0f;

    void OnControllerColliderHit(ControllerColliderHit hit) {
        PushB(hit);
    }

    void PushA(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;

        // No rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We use gravity and weight to push things down, we use 
        // our velocity and push power to push things other directions
        Vector3 force;
        if (hit.moveDirection.y < -0.3f)
        {
            force = new Vector3(0, -0.5f, 0) * Physics.gravity.y * weight;
        }
        else
        {
            force = hit.controller.velocity * pushPower;
        }

        // Apply the push
        body.AddForceAtPosition(force, hit.point);
    }
    
    void PushB(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower / (Mathf.Sqrt(body.mass) );
        //body.AddForce(pushDir*Time.deltaTime*pushPower*1000);
    }
}
