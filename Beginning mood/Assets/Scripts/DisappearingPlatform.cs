using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour {

    [NonSerialized]
    public ParticleSystem[] myParticles = new ParticleSystem[2];

    public bool visibleToDroneCam = true;

    private float gridSize = 1f; // Grid spacing
    private Vector3[] surfacePoints;
    
    private void Start() {
        BoxCollider box = GetComponent<BoxCollider>();
        int totalPoints = CalculateTotalPoints(box, gridSize);
        surfacePoints = new Vector3[totalPoints];
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SetVisibleState(IsVisibleToCamera(ExternalCameraReference.s._camera, gameObject, ExternalCameraReference.s.renderLayerMask) == visibleToDroneCam);
        //SetVisibleState(_renderer.isVisible);
    }

    public bool isVisible = true;
    void SetVisibleState(bool _isVisible) {
        if (_isVisible != isVisible) {
            isVisible = _isVisible;
            if (isVisible) {
                //myParticles[0].Play();
                //myParticles[1].Play();
                GetComponent<MeshRenderer>().enabled = true;
                GetComponent<BoxCollider>().enabled = true;
            } else {
                //myParticles[0].Stop();
                //myParticles[1].Stop();
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
    
    
    
    
    /// <summary>
    /// Checks if an object is visible to a specific camera, including occlusion checks.
    /// Uses the entire bounds of the object rather than just checking its center point.
    /// </summary>
    /// <param name="camera">The camera to check visibility from</param>
    /// <param name="target">The target object with a renderer to check visibility for</param>
    /// <param name="occlusionMask">Optional layer mask for occlusion testing. Default includes all layers.</param>
    /// <returns>True if any part of the object is visible to the camera</returns>
    public  bool IsVisibleToCamera(Camera camera, GameObject target, LayerMask? occlusionMask = null)
    {
        // Ensure required components exist
        if (camera == null || target == null || !camera.enabled)
            return false;

        BoxCollider collider = target.GetComponent<BoxCollider>();
        if (collider == null)
            return false;

        if (Vector3.Distance(camera.transform.position, target.transform.position) < 5) {
            return true; // always visible if close to camera
        }

        // Get the bounds of the object
        var colState = collider.enabled;
        collider.enabled = true;
        Bounds bounds = collider.bounds;
        GenerateBoxColliderSurfaceGrid(collider, gridSize, surfacePoints);
        collider.enabled = colState;
        
        // Check if the object is within the camera's view frustum
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        if (!GeometryUtility.TestPlanesAABB(frustumPlanes, bounds))
            return false;
        
        // Get the corners of the bounding box for more accurate visibility check
        
        // Use actual layer mask or default to everything
        int layerMask = occlusionMask.HasValue ? occlusionMask.Value : Physics.DefaultRaycastLayers;
        
        // Check if any corner of the bounding box is visible (not occluded)
        for (int i = 0; i < surfacePoints.Length; i++) {
            Vector3 corner = surfacePoints[i];
            Vector3 directionToCorner = corner - camera.transform.position;
            float distanceToCorner = directionToCorner.magnitude;

            // Check for occlusion using raycast
            if (!Physics.Raycast(camera.transform.position, directionToCorner, distanceToCorner - 0.01f, layerMask)) {
                // If no hit, the point is visible
                Debug.DrawLine(camera.transform.position, camera.transform.position + (directionToCorner.normalized * (distanceToCorner - 0.01f)), Color.green);
                return true;
            } else {
                Debug.DrawLine(camera.transform.position, camera.transform.position + (directionToCorner.normalized * (distanceToCorner - 0.01f)), Color.red);
            }            
            /*else {
                // Raycast hit something - check if it's our target
                RaycastHit hit;
                if (Physics.Raycast(camera.transform.position, directionToCorner, out hit, distanceToCorner, layerMask)) {
                    if (hit.collider.gameObject == target || hit.collider.transform.IsChildOf(target.transform)) {
                        return true;
                    } else {
                        Debug.DrawLine(camera.transform.position, camera.transform.position + (directionToCorner.normalized * (distanceToCorner - 0.01f)), Color.red);
                    }
                }
            }*/
        }

        // For more accurate testing, check additional points on the object's surface
        // This is optional but helps with larger objects
        /*Vector3[] additionalPoints = GetAdditionalTestPoints(bounds);
        foreach (Vector3 point in additionalPoints)
        {
            Vector3 directionToPoint = point - camera.transform.position;
            float distanceToPoint = directionToPoint.magnitude;
            
            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, directionToPoint, out hit, distanceToPoint, layerMask))
            {
                if (hit.collider.gameObject == target || hit.collider.transform.IsChildOf(target.transform))
                {
                    return true;
                }
            }
        }*/
        
        return false;
    }
    
    /// <summary>
    /// Gets the eight corners of a bounding box
    /// </summary>
    private  Vector3[] GetBoundingBoxCorners(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        
        return new Vector3[]
        {
            center + new Vector3(extents.x, extents.y, extents.z),
            center + new Vector3(extents.x, extents.y, -extents.z),
            center + new Vector3(extents.x, -extents.y, extents.z),
            center + new Vector3(extents.x, -extents.y, -extents.z),
            center + new Vector3(-extents.x, extents.y, extents.z),
            center + new Vector3(-extents.x, extents.y, -extents.z),
            center + new Vector3(-extents.x, -extents.y, extents.z),
            center + new Vector3(-extents.x, -extents.y, -extents.z)
        };
    }
    
     Vector3[] GetBoxColliderCorners(BoxCollider box)
    {
        if (box == null) return null;

        Transform transform = box.transform;
        Vector3 center = box.center;
        Vector3 size = box.size * 0.5f; // Half extents

        // Local-space corner offsets
        Vector3[] localCorners = new Vector3[]
        {
            new Vector3(-size.x, -size.y, -size.z),
            new Vector3(-size.x, -size.y,  size.z),
            new Vector3(-size.x,  size.y, -size.z),
            new Vector3(-size.x,  size.y,  size.z),
            new Vector3( size.x, -size.y, -size.z),
            new Vector3( size.x, -size.y,  size.z),
            new Vector3( size.x,  size.y, -size.z),
            new Vector3( size.x,  size.y,  size.z)
        };

        // Convert to world-space
        Vector3[] worldCorners = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            worldCorners[i] = transform.TransformPoint(center + localCorners[i]);
        }

        return worldCorners;
    }
    
     void GenerateBoxColliderSurfaceGrid(BoxCollider box, float gridSize, Vector3[] outputArray)
    {
        if (box == null || outputArray == null) return;

        Transform transform = box.transform;
        Vector3 center = box.center;
        Vector3 size = box.size * 0.5f; // Half extents

        // Local-space face normals
        Vector3[] faceNormals = new Vector3[]
        {
            Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back
        };

        int index = 0; // Track array index

        foreach (Vector3 normal in faceNormals)
        {
            Vector3 faceCenter = center + normal * GetSizeComponent(size, normal);
            Vector3 axis1 = (normal == Vector3.up || normal == Vector3.down) ? Vector3.right : Vector3.up;
            Vector3 axis2 = Vector3.Cross(normal, axis1).normalized;

            float length1 = GetSizeComponent(size, axis1) * 2;
            float length2 = GetSizeComponent(size, axis2) * 2;

            int steps1 = Mathf.Max(1, Mathf.RoundToInt(length1 / gridSize));
            int steps2 = Mathf.Max(1, Mathf.RoundToInt(length2 / gridSize));

            for (int x = 0; x <= steps1; x++)
            {
                for (int y = 0; y <= steps2; y++)
                {
                    if (index >= outputArray.Length) return; // Prevent overflow

                    Vector3 localPoint = faceCenter
                                         + axis1 * ((x / (float)steps1) * length1 - GetSizeComponent(size, axis1))
                                         + axis2 * ((y / (float)steps2) * length2 - GetSizeComponent(size, axis2));

                    outputArray[index++] = transform.TransformPoint(localPoint);
                }
            }
        }
    }
    
     int CalculateTotalPoints(BoxCollider box, float gridSize)
    {
        if (box == null) return 0;

        Vector3 size = box.size * 0.5f;
        Vector3[] faceNormals = new Vector3[]
        {
            Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back
        };

        int totalPoints = 0;

        foreach (Vector3 normal in faceNormals)
        {
            Vector3 axis1 = (normal == Vector3.up || normal == Vector3.down) ? Vector3.right : Vector3.up;
            Vector3 axis2 = Vector3.Cross(normal, axis1).normalized;

            float length1 = GetSizeComponent(size, axis1) * 2;
            float length2 = GetSizeComponent(size, axis2) * 2;

            int steps1 = Mathf.Max(1, Mathf.RoundToInt(length1 / gridSize));
            int steps2 = Mathf.Max(1, Mathf.RoundToInt(length2 / gridSize));

            totalPoints += (steps1 + 1) * (steps2 + 1);
        }

        return totalPoints;
    }

     float GetSizeComponent(Vector3 size, Vector3 axis)
    {
        return Mathf.Abs(axis.x * size.x + axis.y * size.y + axis.z * size.z);
    }
    
    /// <summary>
    /// Gets additional test points on the faces of the bounding box for more accurate testing
    /// </summary>
    private  Vector3[] GetAdditionalTestPoints(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        
        // Get center points of each face of the bounding box
        return new Vector3[]
        {
            center + new Vector3(extents.x, 0, 0),  // Center of right face
            center + new Vector3(-extents.x, 0, 0), // Center of left face
            center + new Vector3(0, extents.y, 0),  // Center of top face
            center + new Vector3(0, -extents.y, 0), // Center of bottom face
            center + new Vector3(0, 0, extents.z),  // Center of front face
            center + new Vector3(0, 0, -extents.z)  // Center of back face
        };
    }
}
