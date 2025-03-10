using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartDrone : MonoBehaviour, IControllableBody
{
	public float speed = 10;
		
    	private bool doShake = true;
    
    	public Vector3 currentFlightVelocity;

        public Transform followTransform;
        public Transform mainPlayerCam;

        private bool moveBackToPos = true;

        private Rigidbody _rigidbody;
        private void Start() {
	        _audioPlayer = GetComponentInChildren<AudioPlayer>();
	        moveBackToPos = true;
	        _rigidbody = GetComponent<Rigidbody>();
        }

        private HolderRigidbody _holderRigidbody;
        private AudioPlayer _audioPlayer;
        private bool beingControlled = false;

        public bool followPlayer = false;
        public void EnterBody() {
	        moveBackToPos = false;
	        beingControlled = true;
	        _audioPlayer.Play();

	        if (AlphaClippingController.s != null) {
		        AlphaClippingController.s.DisableAlphaClipping();
	        }
	        
	        // do nothing
	        /*_holderRigidbody = new HolderRigidbody(GetComponent<Rigidbody>());
	        Destroy(GetComponent<Rigidbody>());*/
        }

        private void Update() {
	        if (followPlayer && moveBackToPos && followTransform != null) {
		        
		        //transform.position = Vector3.MoveTowards(transform.position, followTransform.position, 5 * Time.deltaTime);

		        var dirVector = followTransform.position - transform.position;
		        _rigidbody.velocity = (dirVector).normalized * Mathf.Min(dirVector.magnitude,5);
		        
		        if (Vector3.Distance(transform.position, followTransform.position) > 0.01f) {
			        _audioPlayer.Play();
		        } else {
			        _audioPlayer.Stop();
		        }
		        
		        transform.rotation = Quaternion.Lerp(transform.rotation, mainPlayerCam.rotation, 2 * Time.deltaTime);
	        }
        }

        public void BodyUpdate(BodyInput bodyInput) {
	        var drone = gameObject;

            var droneLookDirection = bodyInput.moveAxis.forward;
            droneLookDirection.y = 0;
            
            drone.transform.rotation = Quaternion.Lerp(drone.transform.rotation, Quaternion.LookRotation(droneLookDirection), 20 * Time.deltaTime);
    
            currentFlightVelocity = CalculateFlightVelocity(drone.transform, currentFlightVelocity, bodyInput.moveInput, bodyInput.moveAxis);
            
            _rigidbody.velocity = currentFlightVelocity;
            
            //drone.transform.position += currentFlightVelocity * Time.deltaTime;
            
            _audioPlayer.SetPitch(currentFlightVelocity.magnitude/10f + 1f);
            
            /*drone.transform.position += followTransform.position-lastFollowPos;
            lastFollowPos = followTransform.position;*/
        }

        public Vector3 CalculateFlightVelocity(Transform droneTransform, Vector3 previousVelocity, Vector3 wasdInput, Transform cameraTransform)
    	{
    		// Get forward and right vectors based on camera's orientation
    		Vector3 forward = cameraTransform.forward;
    		Vector3 right = cameraTransform.right;
    
    		forward.y = 0f; // Ignore vertical component
    		right.y = 0f;
    
    		forward.Normalize();
    		right.Normalize();
    
    		// Calculate the movement direction based on input
    		Vector3 moveDirection = (forward * wasdInput.z + right * wasdInput.x).normalized;
    
    		// Calculate the horizontal velocity change
    		Vector3 horizontalVelocityChange = moveDirection  * 100;
    
    		// Calculate the vertical velocity change
    		float verticalVelocityChange = wasdInput.y * 200;
    
    		var verticalSpeed = previousVelocity.y;
    		previousVelocity.y = 0;
    
    		// Apply acceleration changes
    		previousVelocity += horizontalVelocityChange*Time.deltaTime;
    		verticalSpeed += verticalVelocityChange*Time.deltaTime;
    
    		// Apply friction
    		previousVelocity = Vector3.Lerp(previousVelocity, Vector3.zero, 4f * Time.deltaTime);
    		verticalSpeed = Mathf.Lerp(verticalSpeed, 0, 12 * Time.deltaTime);
    
    		// Clamp velocity to maximum flight speed
    		previousVelocity = Vector3.ClampMagnitude(previousVelocity, 1.5f*speed);
    		verticalSpeed = Mathf.Clamp(verticalSpeed, -1*speed, 1*speed);
    
    		previousVelocity.y = verticalSpeed;

            if (followTransform != null && !beingControlled) {
	            var repairDronePos = droneTransform.position;
	            var minDistance = float.MaxValue;
	            var minDistanceMoveVector = Vector3.zero;

	            var pos = followTransform.position;
	            var towardsVector = pos - repairDronePos;
	            if (towardsVector.magnitude < minDistance) {
		            minDistance = towardsVector.magnitude;
		            minDistanceMoveVector = towardsVector;
	            }
	            
	            previousVelocity += minDistanceMoveVector.normalized * 1.5f;
            }

            return previousVelocity;
    	}

        public void ExitBody() {
	        moveBackToPos = true;
	        currentFlightVelocity = Vector3.zero;
	        beingControlled = false;
	        
	        if (AlphaClippingController.s != null) {
		        AlphaClippingController.s.EnableAlphaClipping();
	        }
        }
}
