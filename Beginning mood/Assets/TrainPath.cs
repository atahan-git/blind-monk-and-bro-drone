using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPath : MonoBehaviour {
    public TrainPathPoint[] points;

    private void Awake() {
        points = GetComponentsInChildren<TrainPathPoint>();
    }

    public struct PathInfo {
        public Vector3 point;
        public bool canMoveHere;
        public float speedMultiplier;
    }
    
    public PathInfo GetPointOnPath(float distance) {
        if (points == null || points.Length < 2) return new PathInfo { point = Vector3.zero, canMoveHere = true , speedMultiplier =  1};

        if (distance < 0) {
            return new PathInfo { point = points[0].transform.position, canMoveHere = true,speedMultiplier = points[0].speedMultiplier};
        }
        
        float totalLength = 0f;
        float[] segmentLengths = new float[points.Length - 1];
        
        // Calculate segment lengths and total path length
        for (int i = 0; i < points.Length - 1; i++)
        {
            segmentLengths[i] = Vector3.Distance(points[i].transform.position, points[i + 1].transform.position);
            totalLength += segmentLengths[i];
        }
        
        // Clamp distance to total path length
        distance = Mathf.Clamp(distance, 0f, totalLength);
        
        // Find which segment the given distance falls into
        float accumulatedLength = 0f;
        for (int i = 0; i < segmentLengths.Length; i++)
        {
            if (accumulatedLength + segmentLengths[i] >= distance)
            {
                float segmentProgress = (distance - accumulatedLength) / segmentLengths[i];
                var point = Vector3.Lerp(points[i].transform.position, points[i + 1].transform.position, segmentProgress);
                return new PathInfo{point = point, canMoveHere = points[i].canMoveHere, speedMultiplier = points[i].speedMultiplier};
            }
            accumulatedLength += segmentLengths[i];
        }
        
        var point2 = points[points.Length - 1].transform.position;
        return new PathInfo{point = point2, canMoveHere = points[points.Length-1].canMoveHere, speedMultiplier = points[points.Length-1].speedMultiplier}; // Shouldn't reach here, but as a safeguard
    }
}
