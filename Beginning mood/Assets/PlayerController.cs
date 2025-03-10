using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    IControllableBody myCurrentBody;
    IControllableCamera myCurrentCamera;
    InventoryController myCurrentInventory;

    public BodyInput bodyInput;
    public CameraInput cameraInput;
    public InteractInput interactInput;

    public static Camera mainCam;
    public static PlayerController mainPlayer;

    private void Awake() {
        mainPlayer = this;
    }

    private HolderRigidbody _holderRigidbody;
    public void ResetPlayerBody() {
        var _myCurrentBody = (myCurrentBody as MonoBehaviour);
        _holderRigidbody = new HolderRigidbody(_myCurrentBody.GetComponent<Rigidbody>());
        DestroyImmediate(_myCurrentBody.GetComponent<Rigidbody>());
        var rg =_myCurrentBody.gameObject.AddComponent<Rigidbody>();
        _holderRigidbody.Apply(rg);
        if (myCurrentBody is MovingSphere movingSphere) {
            movingSphere.body = rg;
        }
    }

    void Update() {
        if (skipNextUpdate) {
            skipNextUpdate = false;
            return;
        }
        bodyInput.moveInput.x = Input.GetAxis("Horizontal");
        bodyInput.moveInput.z = Input.GetAxis("Vertical");
        bodyInput.moveInput.y = Input.GetAxis("UpDown");

        bodyInput.sprint = Input.GetKey(KeyCode.LeftShift);

        bodyInput.jump = Input.GetButtonDown("Jump");
        bodyInput.climb = Input.GetButton("Climb");
        

        if (myCurrentBody != null) {
            myCurrentBody.BodyUpdate(bodyInput);
        }
        
        int numInput = -1;
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString())) {
                numInput = i;
                break;
            }
        }

        interactInput.actingController = this;
        interactInput.interactDown = Input.GetKeyDown(KeyCode.E);
        interactInput.primaryDown = Input.GetMouseButtonDown(0);
        interactInput.primaryUp = Input.GetMouseButtonUp(0);
        interactInput.scrollDelta = Input.mouseScrollDelta.y;
        interactInput.secondaryDown = Input.GetMouseButtonDown(1);
        interactInput.secondaryUp = Input.GetMouseButtonUp(1);
        interactInput.numInput = numInput;
        interactInput.escape = Input.GetKeyDown(KeyCode.Escape);
        interactInput.droneKey = Input.GetKeyDown(KeyCode.F);
        
        
        myCurrentInventory.Interact(interactInput);
    }

    private void LateUpdate() {
        cameraInput.moveInput = new Vector2(
            Input.GetAxis("Vertical Camera"),
            Input.GetAxis("Horizontal Camera")
        );
        if (myCurrentCamera != null) {
            myCurrentCamera.LateCameraUpdate(cameraInput);
        }
    }

    public ControllableBodyStruct currentBody;
    private bool skipNextUpdate = false;
    public void AssumeBody(ControllableBodyStruct body) {
        if (currentBody != null && currentBody.cam != null) {
            currentBody.cam.GetComponent<Camera>().enabled = false;
            currentBody.cam.GetComponent<AudioListener>().enabled = false;
        }

        if (myCurrentInventory != null) {
            myCurrentInventory.DisableAllTools();
        }

        if (myCurrentBody != null) {
            myCurrentBody.ExitBody();
        }

        if (body.body != null) {
            myCurrentBody = body.body.GetComponent<IControllableBody>();
            myCurrentBody.EnterBody();
        } else {
            myCurrentBody = null;
        }
        myCurrentCamera = body.cam.GetComponent<IControllableCamera>();
        transform.SetParent(body.cam.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        bodyInput.moveAxis = body.cam.transform;
        interactInput.interactSource = body.cam.transform;
        myCurrentInventory = body.inventory.GetComponent<InventoryController>();
        currentBody = body;
        
        currentBody.cam.GetComponent<Camera>().enabled = true;
        currentBody.cam.GetComponent<AudioListener>().enabled = true;
        skipNextUpdate = true;

        mainCam = currentBody.cam.GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

[Serializable]
public struct BodyInput {
    public Transform moveAxis;
    public Vector3 moveInput;
    public bool sprint;
    public bool jump;
    public bool climb;
}

[Serializable]
public struct CameraInput {
    public Vector2 moveInput;
}

[Serializable]
public struct InteractInput {
    public PlayerController actingController;
    public Transform interactSource;
    public bool interactDown;
    public bool primaryDown;
    public bool primaryUp;
    public float scrollDelta;
    public bool secondaryDown;
    public bool secondaryUp;
    public int numInput;
    public bool escape;
    public bool droneKey;
}


public interface IControllableBody {
    public void EnterBody();
    public void BodyUpdate(BodyInput bodyInput);
    public void ExitBody();
}

public interface IControllableCamera {
    public void LateCameraUpdate(CameraInput cameraInput);
}