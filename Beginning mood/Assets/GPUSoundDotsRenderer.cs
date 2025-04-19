using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUSoundDotsRenderer : MonoBehaviour {
    public static GPUSoundDotsRenderer s;
    
    public bool doRender = true;

    private void Awake() {
        s = this;
    }

    [Header("Rendering Settings")]
    [SerializeField] private Mesh instanceMesh;
    [SerializeField] private Material instanceMaterial;
    [SerializeField] private Vector3 meshScale = new Vector3(0.1f, 0.1f, 0.1f);

    private const int BATCH_SIZE = 1023;
    private List<Matrix4x4[]> batchMatricesPool = new List<Matrix4x4[]>();
    private List<int> batchFilledCounts = new List<int>();

    private class DotList
    {
        public int batchStartIndex;
        public int count;
        public float expireTime;
    }

    private List<DotList> activeDotLists = new List<DotList>();

    private void Update()
    {
        float now = Time.time;
        for (int i = activeDotLists.Count - 1; i >= 0; i--)
        {
            if (now >= activeDotLists[i].expireTime)
            {
                RemoveMatrixRange(activeDotLists[i]);
                activeDotLists.RemoveAt(i);
            }
        }
    }

    private void LateUpdate() {
        if (doRender) {
            RenderDots();
        }
    }

    public void AddDotList(List<Vector3> dots, float aliveTime)
    {
        DotList newList = new DotList
        {
            batchStartIndex = 0,
            count = dots.Count,
            expireTime = Time.time + aliveTime
        };

        int remaining = dots.Count;
        int dotIndex = 0;

        for (int b = 0; b < batchMatricesPool.Count && remaining > 0; b++)
        {
            int available = BATCH_SIZE - batchFilledCounts[b];
            int toWrite = Mathf.Min(remaining, available);

            for (int i = 0; i < toWrite; i++)
            {
                batchMatricesPool[b][batchFilledCounts[b]++] = Matrix4x4.TRS(dots[dotIndex++], Quaternion.identity, meshScale);
            }

            remaining -= toWrite;
        }

        while (remaining > 0)
        {
            int toWrite = Mathf.Min(remaining, BATCH_SIZE);
            Matrix4x4[] newBatch = new Matrix4x4[BATCH_SIZE];

            for (int i = 0; i < toWrite; i++)
            {
                newBatch[i] = Matrix4x4.TRS(dots[dotIndex++], Quaternion.identity, meshScale);
            }

            batchMatricesPool.Add(newBatch);
            batchFilledCounts.Add(toWrite);

            remaining -= toWrite;
        }

        activeDotLists.Add(newList);
    }

    private void RemoveMatrixRange(DotList dotList)
    {
        int remaining = dotList.count;
        for (int b = batchMatricesPool.Count - 1; b >= 0 && remaining > 0; b--)
        {
            int remove = Mathf.Min(remaining, batchFilledCounts[b]);
            batchFilledCounts[b] -= remove;
            remaining -= remove;
        }
    }

    private void RenderDots()
    {
        if (instanceMesh == null || instanceMaterial == null)
            return;

        int blindsightLayer = LayerMask.NameToLayer("Blindsight");
        if (blindsightLayer == -1)
        {
            Debug.LogWarning("Blindsight layer not found. Using default layer.");
            blindsightLayer = 0;
        }

        MaterialPropertyBlock props = new MaterialPropertyBlock();

        for (int b = 0; b < batchMatricesPool.Count; b++)
        {
            int count = batchFilledCounts[b];
            if (count == 0) continue;

            Graphics.DrawMeshInstanced(instanceMesh, 0, instanceMaterial, batchMatricesPool[b], count, props,
                UnityEngine.Rendering.ShadowCastingMode.On, true, blindsightLayer);
        }
    }

    private void EnsureBatchPoolSize(int matrixCount)
    {
        int requiredBatchCount = Mathf.CeilToInt((float)matrixCount / BATCH_SIZE);
        while (batchMatricesPool.Count < requiredBatchCount)
        {
            batchMatricesPool.Add(new Matrix4x4[BATCH_SIZE]);
            batchFilledCounts.Add(0);
        }
    }
}
