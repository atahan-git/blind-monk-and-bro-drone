using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetGun : MonoBehaviour, IPlayerTool {
    public LayerMask layerMask;

    public GameObject magnetPrefab;

    public GameObject preMagnet;
    public Rigidbody preMagnetBody;

    public List<GameObject> activeMagnets;
    public List<SpringJoint> joints;
    
    public bool Interact(InteractInput interactInput) {
        bool didHit = false;
        Ray ray = new Ray(interactInput.interactSource.position, interactInput.interactSource.forward);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 20, layerMask)) {
            didHit = true;
            if (interactInput.primaryDown) {
                var magnet = Instantiate(magnetPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), hitInfo.transform);

                /*if (hitInfo.rigidbody != null) {
                    magnet.transform.SetParent(hitInfo.rigidbody.transform);
                }*/

                if (preMagnet == null) {
                    preMagnet = magnet;
                    preMagnetBody = hitInfo.rigidbody;
                } else {
                    activeMagnets.Add(preMagnet);
                    activeMagnets.Add(magnet);

                    var rigid1 = preMagnetBody;
                    var rigid2 = hitInfo.rigidbody;

                    if (rigid1 == null) {
                        rigid1 = rigid2;
                        rigid2 = null;
                    } else {
                        var yeet = preMagnet;
                        preMagnet = magnet;
                        magnet = yeet;
                    }

                    if (rigid1 == null) {
                        preMagnet = null;
                        return true;
                    }

                    var spring = rigid1.gameObject.AddComponent<SpringJoint>();
                    joints.Add(spring);

                    spring.autoConfigureConnectedAnchor = false;
                    spring.enableCollision = true;
                    spring.anchor = magnet.transform.localPosition;
                    magnet.gameObject.name = "spring magnet";

                    var magnetLine = magnet.GetComponentInChildren<MagnetLine>();
                    magnetLine.SetTargets(magnet, preMagnet);


                    preMagnet.gameObject.name = "other magnet";

                    if (rigid2 == null) {
                        rigid2 = preMagnet.AddComponent<Rigidbody>();
                        rigid2.isKinematic = true;
                        rigid2.useGravity = false;

                        spring.connectedBody = rigid2;
                        spring.connectedAnchor = Vector3.zero;
                    } else {
                        spring.connectedBody = rigid2;
                        spring.connectedAnchor = preMagnet.transform.localPosition;
                    }


                    preMagnet = null;
                }
            }
        } 


        if (interactInput.secondaryDown) {

            if (preMagnet != null) {
                Destroy(preMagnet);
            }

            if (joints.Count > 0) {
                Destroy(joints[0]);
                joints.RemoveAt(0);
                Destroy(activeMagnets[0]);
                Destroy(activeMagnets[1]);
                activeMagnets.RemoveAt(0);
                activeMagnets.RemoveAt(0);
            }
        }

        return didHit;
    }

    public bool HoldAttention() {
        return false;
    }

    public void DisableTool() {
        // do nothing
    }
    

}
