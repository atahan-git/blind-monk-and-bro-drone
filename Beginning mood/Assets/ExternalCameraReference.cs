using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalCameraReference : MonoBehaviour
{
    public LayerMask renderLayerMask;
    public Camera _camera;
    public static ExternalCameraReference s;
    
    private void Awake() {
        s = this;
        _camera = GetComponent<Camera>();
    }
}
