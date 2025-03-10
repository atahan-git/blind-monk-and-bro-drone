using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour {

    [NonSerialized]
    public ParticleSystem[] myParticles = new ParticleSystem[2];

    public bool visibleToDroneCam = true;
    

    // Update is called once per frame
    void Update()
    {
        SetVisibleState(IsVisibleToCamera(ExternalCameraReference.s._camera, gameObject, ExternalCameraReference.s.renderLayerMask) == visibleToDroneCam);
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
    public static bool IsVisibleToCamera(Camera camera, GameObject target, LayerMask? occlusionMask = null)
    {
        // Ensure required components exist
        if (camera == null || target == null)
            return false;

        Collider collider = target.GetComponent<Collider>();
        if (collider == null)
            return false;

        // Get the bounds of the object
        var colState = collider.enabled;
        collider.enabled = true;
        Bounds bounds = collider.bounds;
        collider.enabled = colState;
        
        // Check if the object is within the camera's view frustum
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        if (!GeometryUtility.TestPlanesAABB(frustumPlanes, bounds))
            return false;
        
        // Get the corners of the bounding box for more accurate visibility check
        Vector3[] boundingBoxCorners = GetBoundingBoxCorners(bounds);
        
        // Use actual layer mask or default to everything
        int layerMask = occlusionMask.HasValue ? occlusionMask.Value : Physics.DefaultRaycastLayers;
        
        // Check if any corner of the bounding box is visible (not occluded)
        foreach (Vector3 corner in boundingBoxCorners)
        {
            Vector3 directionToCorner = corner - camera.transform.position;
            float distanceToCorner = directionToCorner.magnitude;
            
            // Check for occlusion using raycast
            if (!Physics.Raycast(camera.transform.position, directionToCorner, distanceToCorner-0.01f, layerMask))
            {
                // If no hit, the point is visible
                return true;
            } 
            /*else
            {
                // Raycast hit something - check if it's our target
                RaycastHit hit;
                if (Physics.Raycast(camera.transform.position, directionToCorner, out hit, distanceToCorner, layerMask))
                {
                    if (hit.collider.gameObject == target || hit.collider.transform.IsChildOf(target.transform))
                    {
                        return true;
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
    private static Vector3[] GetBoundingBoxCorners(Bounds bounds)
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
    
    /// <summary>
    /// Gets additional test points on the faces of the bounding box for more accurate testing
    /// </summary>
    private static Vector3[] GetAdditionalTestPoints(Bounds bounds)
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
