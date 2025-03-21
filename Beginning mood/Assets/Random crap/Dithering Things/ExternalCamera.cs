using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class ExternalCamera : MonoBehaviour
{
    private static readonly int ExternalCameraMatrix = Shader.PropertyToID("_ExternalCameraMatrix");
    private static readonly int ExternalCameraDepth = Shader.PropertyToID("_ExternalCameraDepth");
    private static readonly int Dither = Shader.PropertyToID("_Dither");
    [SerializeField] private Texture2D ditherTexture;
    new Camera camera;
    UniversalAdditionalCameraData cameraData;
    RenderTexture renderTexture;

    public bool doRenderTexture = true;

    private void Start() {
        if (ditherTexture)
        {
            Shader.SetGlobalTexture(Dither, ditherTexture);
        }
    }

    void LateUpdate()
    {
        if (!camera)
        {
            camera = GetComponent<Camera>();
            cameraData = GetComponent<UniversalAdditionalCameraData>();
            if (!camera) return;
        }
        
        Shader.SetGlobalMatrix(ExternalCameraMatrix, camera.nonJitteredProjectionMatrix * camera.worldToCameraMatrix);
        if (doRenderTexture) {
            camera.enabled = true;
            Shader.SetGlobalTexture(ExternalCameraDepth, camera.targetTexture);
        } else {
            camera.enabled = false;
            Shader.SetGlobalTexture(ExternalCameraDepth, null);
        }

    }
}
