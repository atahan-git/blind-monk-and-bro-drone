using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUInstancedDotRenderer : MonoBehaviour
{
    [Header("Rendering Settings")]
    [SerializeField] private Mesh instanceMesh;
    [SerializeField] private Material instanceMaterial;
    [SerializeField] private Vector3 meshScale = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private int historyFrameCount = 4;
    
    // Structure to hold a frame's worth of dot data
    private class DotCollection {
        public DotFrame[] dotFrames;
    }
    private class DotFrame
    {
        public List<Matrix4x4> matrices = new List<Matrix4x4>();
    }
    
    // Circular buffer of dot frames
    private DotCollection[] dotCollections;
    private int currentFrameIndex = 0;
    
    // Cache for batch arrays to prevent GC allocation
    private List<Matrix4x4[]> batchMatricesPool = new List<Matrix4x4[]>();
    private const int BATCH_SIZE = 1023; // Maximum instances per batch

    private const int frameCount = 100;

    public bool setupComplete = false;
    public void Awake()
    {
        if (setupComplete) {
            return;
        }
        // Initialize the history frames
        dotCollections = new DotCollection[historyFrameCount];
        for (int i = 0; i < historyFrameCount; i++)
        {
            dotCollections[i] = new DotCollection
            {
                dotFrames = new DotFrame[frameCount]
            };

            for (int j = 0; j < frameCount; j++) {
                dotCollections[i].dotFrames[j] = new DotFrame() {
                    matrices = new List<Matrix4x4>(1000)
                };
            }
        }

        setupComplete = true;
    }

    public void ClearAllDots() {
        clearByFrameAll = true;
        clearFrameByFrameAllIndex = 0;
        
        /*for (int j = 0; j < historyFrameCount; j++) {
            for (int i = 0; i < dotCollections[j].dotFrames.Length; i++) {
                dotCollections[j].dotFrames[i].matrices.Clear();
            }
        }*/
        
    }
    
    public void ClearDots() {
        if (!setupComplete) {
            return;
        }
        // Move to the next frame in our circular buffer
        currentFrameIndex = (currentFrameIndex + 1) % historyFrameCount;
        
        // Clear only the current frame - keep previous frames intact
        for (int i = 0; i < dotCollections[currentFrameIndex].dotFrames.Length; i++) {
            dotCollections[currentFrameIndex].dotFrames[i].matrices.Clear();
        }

        if (historyFrameCount > 1) {
            clearFrameByFrameIndex = (currentFrameIndex + 1) % historyFrameCount;
            clearFrameByFrame = true;
            clearFrameByFrameCurFrame = 0;
        }

        curFrameIndex = 0;
    }

    public int clearFrameByFrameIndex;
    public bool clearFrameByFrame;
    public int clearFrameByFrameCurFrame;
    
    public bool clearByFrameAll;
    public int clearFrameByFrameAllIndex;
    private void FixedUpdate() {
        if (!setupComplete) {
            return;
        }

        if (clearByFrameAll) {
            for (int i = 0; i < dotCollections.Length; i++) {
                if (clearFrameByFrameAllIndex < frameCount) {
                    dotCollections[i].dotFrames[clearFrameByFrameAllIndex].matrices.Clear();
                } 

                
            }
            clearFrameByFrameAllIndex += 1;
            if (clearFrameByFrameAllIndex == frameCount) {
                clearByFrameAll = false;
            }
        }
        
        if (clearFrameByFrame) {
            if (clearFrameByFrameCurFrame < frameCount) {
                dotCollections[clearFrameByFrameIndex].dotFrames[clearFrameByFrameCurFrame].matrices.Clear();
                clearFrameByFrameCurFrame += 1;
            } else {
                clearFrameByFrame = false;
            }
        }

        if (curFrameIndex < frameCount) {
            curFrameIndex += 1;
        }
    }

    public int curFrameIndex = 0;

    public void AddParticleAtPoint(Vector3 position, Vector3 normal) {
        if (!setupComplete) {
            return;
        }
        // Create matrix for this instance - position, rotation (facing normal), and scale
        Matrix4x4 matrix = Matrix4x4.TRS(
            position,
            Quaternion.FromToRotation(Vector3.up, normal),
            meshScale
        );

        // Add to the current frame's data
        if (curFrameIndex < dotCollections[currentFrameIndex].dotFrames.Length) {
            dotCollections[currentFrameIndex].dotFrames[curFrameIndex].matrices.Add(matrix);
        }
    }

    private void LateUpdate()
    {
        RenderDots();
    }
    
    private void RenderDots()
    {
        if (instanceMesh == null || instanceMaterial == null)
            return;
            
        // Get the "Blindsight" layer index
        int blindsightLayer = LayerMask.NameToLayer("Blindsight");
        
        // If the layer doesn't exist, log a warning but continue with default layer
        if (blindsightLayer == -1)
        {
            Debug.LogWarning("Blindsight layer not found. Create it in your project's layer settings. Using default layer instead.");
            blindsightLayer = 0; // Default layer
        }
        
        // Set material properties
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        
        // Render all active frames from history
        for (int collectionOffset = 0; collectionOffset < historyFrameCount; collectionOffset++)
        {
            for (int frameOffset = 0; frameOffset < dotCollections[collectionOffset].dotFrames.Length; frameOffset++) {
                List<Matrix4x4> frameMatrices = dotCollections[collectionOffset].dotFrames[frameOffset].matrices;
            
                // Skip empty frames
                if (frameMatrices.Count == 0)
                    continue;
                
                // For older frames, we could modify color/transparency here
                // props.SetColor(colorID, new Color(dotColor.r, dotColor.g, dotColor.b, dotColor.a * (1f - frameOffset / (float)historyFrameCount)));
            
                // Ensure we have enough batch arrays for all dots in this frame
                EnsureBatchPoolSize(frameMatrices.Count);
            
                // Draw all instances in this frame
                int batchCount = Mathf.CeilToInt((float)frameMatrices.Count / BATCH_SIZE);
            
                for (int i = 0; i < batchCount; i++)
                {
                    int startIndex = i * BATCH_SIZE;
                    int count = Mathf.Min(BATCH_SIZE, frameMatrices.Count - startIndex);
                
                    Matrix4x4[] batchMatrices = batchMatricesPool[i];
                
                    // Copy matrices to the pre-allocated batch array
                    for (int j = 0; j < count; j++)
                    {
                        batchMatrices[j] = frameMatrices[startIndex + j];
                    }
                
                    // Use the Blindsight layer for instanced meshes
                    Graphics.DrawMeshInstanced(instanceMesh, 0, instanceMaterial, batchMatrices, count, props, 
                        UnityEngine.Rendering.ShadowCastingMode.On, true, blindsightLayer);
                }
            }
        }
    }
    
    private void EnsureBatchPoolSize(int matrixCount)
    {
        // Calculate how many batches we need
        int requiredBatchCount = Mathf.CeilToInt((float)matrixCount / BATCH_SIZE);
        
        // Add more batch arrays if needed
        while (batchMatricesPool.Count < requiredBatchCount)
        {
            batchMatricesPool.Add(new Matrix4x4[BATCH_SIZE]);
        }
    }
}

