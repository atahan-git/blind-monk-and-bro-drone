using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour {

    public TrainPath pathToFollow;

    public bool isMoving;


    public void StartMoving() {
        if (!isMoving) {
            isMoving = true;
            //distanceAlongPath = Vector3.Distance(transform.position, pathToFollow.GetPointOnPath(0).point);
            PlayerController.mainPlayer.ResetPlayerBody();
        }
    }

    private Rigidbody rg;
    void Start() {
        if (isMoving) {
            isMoving = false;
            StartMoving();
        }

        _audioPlayer = GetComponentInChildren<AudioPlayer>();
        rg = GetComponent<Rigidbody>();
    }

    public float speed = 5;
    public float acceleration = 2f;
    public float curSpeed = 0;
    public float rotSpeed = 5;
    public float rotAcceleration = 2f;
    public float curRotSpeed = 0;
    public float distanceAlongPath;
    public float trainRotationLength = 5;
    
    private AudioPlayer _audioPlayer;
    void FixedUpdate()
    {
        if (isMoving) {
            var curPos = rg.position;
            var curRot = rg.rotation;
            
            
            /*var curPos = transform.position;
            var curRot = transform.rotation;*/

            var targetLoc = pathToFollow.GetPointOnPath(distanceAlongPath);
            var targetPos = targetLoc.point;
            var forwardLoc = pathToFollow.GetPointOnPath(distanceAlongPath + (trainRotationLength / 2f));
            var forwardPos = forwardLoc.point;
            var backwardsLoc = pathToFollow.GetPointOnPath(distanceAlongPath - (trainRotationLength / 2f));
            var backwardsPos = backwardsLoc.point;
            
            
            var forwardMoveCheckLoc = pathToFollow.GetPointOnPath(distanceAlongPath + (trainRotationLength / 2f) + 0.5f + (curSpeed*curSpeed)/(2*acceleration));
            
            
            targetPos.y = Mathf.Max(targetPos.y, (backwardsPos.y+forwardPos.y)/2f);
            
            
            var delta = forwardPos - backwardsPos;
            var targetRot = curRot;
            if (delta.magnitude > 0) {
                targetRot = Quaternion.LookRotation(delta);
            }

            if (!forwardMoveCheckLoc.canMoveHere) {
                curSpeed = Mathf.MoveTowards(curSpeed, 0, acceleration * Time.deltaTime);
            }else if (Vector3.Distance(targetPos, curPos) > 0.01f || distanceAlongPath >= 0) {
                curSpeed = Mathf.MoveTowards(curSpeed, speed*targetLoc.speedMultiplier, acceleration * Time.deltaTime);
            } else {
                curSpeed = 0.01f;
            }

            if (Quaternion.Angle(targetRot, transform.rotation) > 0.01f) {
                curRotSpeed = Mathf.MoveTowards(curRotSpeed, rotSpeed, rotAcceleration * Time.deltaTime);
            } else {
                curRotSpeed = 0;
            }
            
            rg.MovePosition(targetPos);
            rg.MoveRotation(Quaternion.Slerp(curRot, targetRot, curRotSpeed*Time.deltaTime));
            
            
            /*transform.position = (Vector3.MoveTowards(curPos, targetPos, curSpeed*Time.deltaTime));
            transform.rotation = (Quaternion.Slerp(curRot, targetRot, curRotSpeed*Time.deltaTime));*/
            
            distanceAlongPath += curSpeed * Time.deltaTime;

            if (curSpeed > 0.01f) {
                _audioPlayer.Play();
                var speedPercent = curSpeed / speed;
                if (speedPercent > 1) {
                    speedPercent = ((speedPercent - 1) / 2f) + 1f;
                }
                _audioPlayer.SetPitch(speedPercent/2f);
            } else {
                _audioPlayer.Stop();
            }
        } else {
            _audioPlayer.Stop();
        }
    }

    private void OnDrawGizmos() {
        var targetLoc = pathToFollow.GetPointOnPath(distanceAlongPath);
        var targetPos = targetLoc.point;
        var forwardLoc = pathToFollow.GetPointOnPath(distanceAlongPath + (trainRotationLength / 2f));
        var forwardPos = forwardLoc.point;
        var backwardsLoc = pathToFollow.GetPointOnPath(distanceAlongPath - (trainRotationLength / 2f));
        var backwardsPos = backwardsLoc.point;
        
        Gizmos.color =Color.green;
        Gizmos.DrawWireSphere(targetPos, 1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(forwardPos, 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(backwardsPos, 1f);
        
        
        var forwardMoveCheckLoc = pathToFollow.GetPointOnPath(distanceAlongPath + (trainRotationLength / 2f) + 0.5f + (curSpeed*curSpeed)/(2*acceleration));

        if (forwardMoveCheckLoc.canMoveHere) {
            Gizmos.color = Color.green;
        } else {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireCube(forwardMoveCheckLoc.point,Vector3.one);
    }

    [ContextMenu("jump to current point in path")]
    public void Editor_JumpToFirstPointInPath() {
        pathToFollow.points = pathToFollow.GetComponentsInChildren<TrainPathPoint>();
        transform.position = pathToFollow.GetPointOnPath(distanceAlongPath).point;
        transform.rotation = Quaternion.LookRotation(pathToFollow.GetPointOnPath(distanceAlongPath+1).point-pathToFollow.GetPointOnPath(distanceAlongPath).point);
    }
}
