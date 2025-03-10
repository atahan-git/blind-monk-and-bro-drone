using System;
using UnityEngine;

public class OLD_PlayerController : MonoBehaviour {
    public static OLD_PlayerController s;

    private void Awake() {
        s = this;
    }

    // Cached vars
    CharacterController thisCharacterController;
    Transform cameraTransform;

    // Internal vars
    float InpVer;
    float InpHor;
    float jumpTimer = 0;
    float yRotate;
    Vector3 direction;
    Vector3 directionLerped;

    float sprint = 1;

    // Adjustable vars
    [SerializeField] float speed = 10;
    [SerializeField] float sprintMax = 2;
    [SerializeField] float jumpTime = 0.25f;
    [SerializeField] float jumpSpeed = 8;
    [SerializeField] float gravity = 6;
    [SerializeField] float mouseSpeed = 6;
    [SerializeField] float speedLerp = 1000;
    [SerializeField] bool doLerp = false;
    
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private float jumpVelo = 0;

    private bool jump = false;


    public bool canMove = true;
    public bool canLook = true;
    // Start is called before the first frame update
    void Start() {
        // Setup character controller
        thisCharacterController = gameObject.GetComponent<CharacterController>();
        // Setup camera
        cameraTransform = Camera.main.transform;
        cameraTransform.position = transform.position + Vector3.up * 1.75f;
        cameraTransform.transform.parent = transform;
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetCanMoveState(bool state) {
        canMove = state;

        thisCharacterController.enabled = canMove;
    }

    
    void Update() {

        if (canMove) {

            // Get Input
            InpHor = Input.GetAxis("Horizontal");
            InpVer = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.LeftShift)) {
                if (sprint < sprintMax) sprint += Time.deltaTime * 5;
            } else {
                if (sprint > 1) sprint -= Time.deltaTime * 5;
            }

            
            if (thisCharacterController.isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }
            
            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }

            // Get Jump
            if (!jump && jumpBufferCounter > 0 && coyoteTimeCounter>0) {
                // Make jump timer = to jump time
                jumpBufferCounter = 0f;
                jumpTimer = jumpTime;
                jump = true;
                //jumpVelo = 20;
            }

            if (!Input.GetButton("Jump")) {
                jump = false;
            }
        }

        if (canLook) {
            // Rotate
            transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSpeed, 0);
        }

        if (canMove) {
            // Construct input vector
            direction = (transform.forward * InpVer) + (transform.right * InpHor);
            if (doLerp) {
                directionLerped = Vector3.Lerp(directionLerped, direction, speedLerp * Time.deltaTime);
                direction = directionLerped;
            }

            direction *= speed * sprint;

            // If we are above zero move up, Jump
            if (jumpTimer > 0) {
                // decrease jumpTimer
                if (jump) {
                    jumpTimer -= Time.deltaTime;
                } else {
                    jumpTimer -= Time.deltaTime * 2;
                }

                direction.y += jumpSpeed * jumpTimer;
                //print(jumpSpeed * jumpTimer);
            } else {
                jumpTimer -= Time.deltaTime;
                if (thisCharacterController.isGrounded) {
                    jumpTimer = 0;
                } else if (jumpTimer < -1f) {
                    jumpTimer = -1f;
                }

                direction.y += gravity * jumpTimer;
                //print(gravity * jumpTimer);
            }

            // Move controller
            thisCharacterController.Move(direction * Time.deltaTime);
        }

        if (canLook) {
            // Camera rotate up down
            yRotate += -Input.GetAxis("Mouse Y") * mouseSpeed;
            // Clamp rotation
            yRotate = Mathf.Clamp(yRotate, -85, 89);
            // Apply rotation
            cameraTransform.localEulerAngles = new Vector3(yRotate, 0, 0);
        }
    }

    private void FixedUpdate() { }
    
}