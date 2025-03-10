using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;

public class ManipulatorBeam : MonoBehaviour, IPlayerTool {


    public ObjectSelectionHelper<Rigidbody> selector = new ObjectSelectionHelper<Rigidbody>();
    public LayerMask layerMask;

    public HighlightProfile highlightProfile;
    public HighlightProfile selectProfile;

    public bool isHolding = false;
    public float holdDistance = 0;

    private InteractInput lastInput;

    public Transform rotator;

    public float range = 20;
    private AudioPlayer _audioPlayer;

    private void Start() {
        _audioPlayer = GetComponentInChildren<AudioPlayer>();
    }

    public bool Interact(InteractInput interactInput)
    {
        if (selector.curObject == null || selector.curObject.gameObject == null) {
            StopHolding();
            selector.curObject = null;
        }

        bool grabAttention = false;
        if (!isHolding) {
            Ray ray = new Ray(interactInput.interactSource.position, interactInput.interactSource.forward);

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, range, layerMask)) {

                Carryable carryable = null;
                if (hitInfo.rigidbody != null) {
                    carryable = hitInfo.rigidbody.GetComponent<Carryable>();
                }
                
                if (carryable) {
                    selector.Select(hitInfo.rigidbody, highlightProfile);

                    if (interactInput.primaryDown || interactInput.secondaryDown) {
                        selector.Select(selector.curObject, selectProfile);
                        StartHolding();
                        holdDistance = Vector3.Distance(transform.position, selector.curObject.transform.position);
                        grabAttention = true;
                    }
                } else {
                    selector.Deselect();
                }
            } else {
                selector.Deselect();
            }
        } else {

            holdDistance += interactInput.scrollDelta*Time.deltaTime*50;
            holdDistance = Mathf.Clamp(holdDistance, 1.4f, range-2);

            if (Vector3.Distance(selector.curObject.transform.position, transform.position) > range + 4) {
                StopHolding();
                grabAttention = true;
            }

            if (interactInput.primaryUp || interactInput.secondaryDown) {
                StopHolding();
                grabAttention = true;
            }
        }

        lastInput = interactInput;

        return selector.curObject != null || grabAttention;
    }

    public bool HoldAttention() {
        return isHolding;
    }

    void StopHolding() {
        isHolding = false;
        if (selector.curObject != null) {
            selector.curObject.drag = selector.curObject.GetComponent<Carryable>().drag;
            selector.curObject.angularDrag = selector.curObject.GetComponent<Carryable>().angularDrag;
            selector.Deselect();
        }
        _audioPlayer.Stop();
    }


    void StartHolding() {
        if (selector.curObject != null) {
            isHolding = true;
            selector.curObject.drag = 10;
            selector.curObject.angularDrag = 10;
            _audioPlayer.Play();
        }
    }

    public void DisableTool() {
        if (!isHolding) {
            if (selector.curObject != null) {
                selector.curObject.drag = selector.curObject.GetComponent<Carryable>().drag;
                selector.curObject.angularDrag = selector.curObject.GetComponent<Carryable>().angularDrag;
                selector.Deselect();
            }
        }
    }

    // PID parameters
    public float proportionalGain = 10f; // Controls the strength of the proportional term
    public float integralGain = 0.5f;    // Controls the strength of the integral term
    public float derivativeGain = 2f;   // Controls the strength of the derivative term

    private Vector3 previousError = Vector3.zero; // Error in the previous frame
    private Vector3 integral = Vector3.zero;      // Cumulative error (integral term)
    
    
    public float proportionalGainQ = 10f; // Controls the strength of the proportional term
    public float integralGainQ = 0.5f;    // Controls the strength of the integral term
    public float derivativeGainQ = 2f;   // Controls the strength of the derivative term
    
    private Vector3 previousErrorQ = Vector3.zero; // Error in the previous frame
    private Vector3 integralQ = Vector3.zero;      // Cumulative error (integral term)
    
    private void FixedUpdate() {
        if (isHolding) {
            /*var targetPos = transform.position + transform.forward * holdDistance;
            var pushVector = targetPos-curObject.position;
            var magnitude = pushVector.magnitude;
            pushVector.Normalize();
            
            curObject.AddForce(magnitude*20000*Time.deltaTime*pushVector);*/
            
            if (selector.curObject == null || selector.curObject.gameObject == null) {
                StopHolding();
                return;
            }

            var curObject = selector.curObject;
            
            var targetPos = lastInput.interactSource.position + lastInput.interactSource.transform.forward * holdDistance + Vector3.down*0.4f;
            // Calculate the error
            Vector3 error = targetPos - curObject.position;

            // Proportional term
            Vector3 proportional = proportionalGain * error;

            // Integral term (accumulate error over time)
            integral += error * Time.fixedDeltaTime;
            Vector3 integralTerm = integralGain * integral;

            // Derivative term (rate of change of error)
            Vector3 derivative = (error - previousError) / Time.fixedDeltaTime;
            Vector3 derivativeTerm = derivativeGain * derivative;

            // Combine all terms to compute the force
            Vector3 force = proportional + integralTerm + derivativeTerm;

            // Apply the force
            var maxForce = Mathf.Min(1200, curObject.mass * 200);
            var clampedForce = force;
            if (force.magnitude > maxForce) {
                clampedForce = force.normalized * maxForce;
            }
            
            _audioPlayer.SetPitch(clampedForce.magnitude.Remap(10,200,1,2));
            
            curObject.AddForce(clampedForce, ForceMode.Force);
            
            rotator.Rotate(Vector3.forward*clampedForce.magnitude*Time.deltaTime);

            // Store the current error for the next frame
            previousError = error;



            var zeroYlook = transform.forward;
            zeroYlook.y = 0;
            var targetRotation = Quaternion.LookRotation(zeroYlook);
            // Calculate the rotation error (in quaternion space)
            Quaternion errorQuaternion = targetRotation * Quaternion.Inverse(curObject.rotation);

            // Extract the rotation axis and angle
            if (errorQuaternion.w < 0) // Handle quaternion double-cover property
            {
                errorQuaternion.x = -errorQuaternion.x;
                errorQuaternion.y = -errorQuaternion.y;
                errorQuaternion.z = -errorQuaternion.z;
                errorQuaternion.w = -errorQuaternion.w;
            }

            Vector3 rotationAxis;
            float rotationAngle;
            errorQuaternion.ToAngleAxis(out rotationAngle, out rotationAxis);

            // Convert the angle to radians and scale the axis
            rotationAngle = Mathf.Deg2Rad * rotationAngle;
             error = rotationAxis * rotationAngle;

            // Proportional term
             proportional = proportionalGainQ * error;

            // Integral term (accumulated error over time)
            integralQ += error * Time.fixedDeltaTime;
             integralTerm = integralGainQ * integralQ;

            // Derivative term (change in error over time)
             derivative = (error - previousErrorQ) / Time.fixedDeltaTime;
             derivativeTerm = derivativeGainQ * derivative;

            // Combine terms to compute the torque
            Vector3 torque = proportional + integralTerm + derivativeTerm;

            // Apply the torque to the Rigidbody
            var clampedTorque = torque;
            if (torque.magnitude > 200) {
                clampedTorque = torque.normalized * 200;
            }
            curObject.AddTorque(clampedTorque, ForceMode.Force);

            // Store the current error for the next frame
            previousErrorQ = error;
        }
    }
    
    // Helper method to convert a quaternion to an angular error in Euler angles
    private Vector3 QuaternionToEuler(Quaternion q)
    {
        q.Normalize(); // Ensure quaternion is normalized
        float angle;
        Vector3 axis;

        q.ToAngleAxis(out angle, out axis);
        return axis * Mathf.Deg2Rad * angle; // Return the angular error in radians
    }

}
