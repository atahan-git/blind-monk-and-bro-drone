using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindSight : MonoBehaviour, IPlayerTool
{
    [Header("Ray Settings")]
    [SerializeField] private float minSphereRadius = 0.1f;
    [SerializeField] private float maxSphereRadius = 10f;
    [SerializeField] private float expansionSpeed = 1f;
    [SerializeField] private float rayLength = 0.5f;
    
    [Header("Resolution Settings")]
    [SerializeField] private int minResolution = 10;
    [SerializeField] private int maxResolution = 100;
    [SerializeField] private AnimationCurve resolutionCurve;
    
    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem particleSystem;
    
    [Header("Layer Settings")]
    [SerializeField] private LayerMask hitLayers;
    
    // Private variables
    private float currentRadius = float.MaxValue;
    private int currentResolution;
    private ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
    
    private GPUInstancedDotRenderer dotRenderer;

    private void Start() {
        dotRenderer = GetComponent<GPUInstancedDotRenderer>();
        dotRenderer.Awake();
        if (resolutionCurve.length == 0)
        {
            // Default curve if none provided
            resolutionCurve = new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(1, 1)
            );
        }
        
        // Make sure we have a particle system
        if (particleSystem == null)
        {
            Debug.LogError("Particle System is not assigned to SphericalRayCaster!");
            enabled = false;
            return;
        }
        
        ResetSphere();
        currentRadius = maxSphereRadius+1;
    }

    public int mode = 0;
    private int modeCount = 3;
    private void FixedUpdate() {
        if (currentRadius <= maxSphereRadius) {
            // Expand the sphere
            currentRadius += expansionSpeed * Time.deltaTime;

            if (currentRadius > 0) {
                // Calculate resolution based on sphere size
                float normalizedRadius = Mathf.InverseLerp(minSphereRadius, maxSphereRadius, currentRadius);
                float resolutionFactor = resolutionCurve.Evaluate(normalizedRadius);
                currentResolution = Mathf.RoundToInt(Mathf.Lerp(minResolution, maxResolution, resolutionFactor));

                // Cast rays
                if (mode == 0) {
                    CastRaysMatrixGridAligned();
                }else if (mode == 1) {
                    CastRaysMatrix();
                }else if (mode == 2) {
                    CastRaysWorldGrid();
                }/*else if (mode == 3) {
                    CastRaysRandom();
                }*/
                /*if (isRandom) {
                    //CastRaysWorldGrid();
                    CastRaysMatrix();
                    //CastRaysRandom();
                } else {
                    CastRaysMatrixGridAligned();
                }*/
            }
        }
    }
    
    private void CastRaysRandom()
    {
        // Using random distribution for points on sphere
        int particlesEmitted = 0;
        
        for (int i = 0; i < currentResolution; i++)
        {
            // Generate random direction for truly random distribution
            Vector3 direction = Random.onUnitSphere;
            
            // Cast ray
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + direction * currentRadius;
            
            Debug.DrawLine(rayOrigin, rayOrigin+direction, Color.red);
            
            if (Physics.Raycast(rayOrigin, direction, out hit, rayLength, hitLayers))
            {
                EmitParticleAtPoint(hit.point, hit.normal);
                particlesEmitted++;
            }
        }
    }

    public float spreadFactor = 3;

    private void CastRaysMatrix() {
        // Calculate grid dimensions based on resolution
        // As resolution increases, we add more points while keeping existing ones
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(currentResolution * 1.5f));

        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                // Convert grid coordinates to normalized coordinates (-1 to 1)
                // This ensures stability - points don't move as resolution increases
                float normalizedX = (x / (gridSize - 1f)) * 2f - 1f;
                float normalizedY = (y / (gridSize - 1f)) * 2f - 1f;

                // Calculate distance from center of grid
                float distFromCenter = Mathf.Sqrt(normalizedX * normalizedX + normalizedY * normalizedY);

                // Skip points outside our circular area (for circular coverage)
                if (distFromCenter > 1.0f)
                    continue;

                // Map the 2D grid point to a direction on a hemisphere facing forward
                Vector3 direction;

                // Equidistant azimuthal projection (Lambert equal-area projection)
                float theta = Mathf.Atan2(normalizedY, normalizedX); // Azimuthal angle
                float r = distFromCenter; // Radial distance (already normalized 0-1)

                // Apply spreadFactor to control cone width
                // Smaller spreadFactor = narrower cone
                r = r * spreadFactor;

                // Convert to spherical coordinates
                // The max angle is now controlled by the spreadFactor
                // spreadFactor of 1.0 means 90 degrees (full hemisphere)
                // spreadFactor of 0.5 means 45 degrees (quarter hemisphere)
                float phi = r * Mathf.PI / 2; // Polar angle (0 at forward, PI/2 at horizon)

                // Convert to Cartesian coordinates (with Z forward)
                float x3D = Mathf.Sin(phi) * Mathf.Cos(theta);
                float y3D = Mathf.Sin(phi) * Mathf.Sin(theta);
                float z3D = Mathf.Cos(phi);

                direction = transform.TransformDirection(new Vector3(x3D, y3D, z3D)).normalized;

                // Cast ray
                RaycastHit hit;
                Vector3 rayOrigin = transform.position + direction * currentRadius;

                Debug.DrawRay(rayOrigin, direction * rayLength, Color.red);

                if (Physics.Raycast(rayOrigin, direction, out hit, rayLength, hitLayers)) {
                    dotRenderer.AddParticleAtPoint(hit.point, hit.normal);
                }
            }
        }
    }
    
    private void CastRaysMatrixGridAligned() {
        var direction = transform.forward;
        var gridResolution = 0.25f;
        Vector3 rayOrigin = transform.position + direction * currentRadius;
        rayOrigin = RoundToDecimal(rayOrigin);

        // Normalize the direction to ensure consistent behavior
        direction = direction.normalized;
    
        // Find perpendicular vectors to create the grid plane
        Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
        if (right.magnitude < 0.001f)
        {
            // If direction is parallel to up, use a different reference vector
            right = Vector3.Cross(direction, Vector3.forward).normalized;
        }
        Vector3 up = Vector3.Cross(right, direction).normalized;
    
        // Calculate grid dimensions
        int gridSize = Mathf.CeilToInt(currentRadius * 2 / gridResolution);
    
        // Cast center ray first
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, direction, out hit, rayLength, hitLayers))
        {
            dotRenderer.AddParticleAtPoint(hit.point, hit.normal);
        }
    
        // Cast grid rays
        for (float x = -currentRadius; x <= currentRadius; x += gridResolution)
        {
            for (float y = -currentRadius; y <= currentRadius; y += gridResolution)
            {
                // Skip the center ray as we already processed it
                if (x == 0 && y == 0) continue;
            
                // Calculate offset from center ray
                Vector3 offset = right * x + up * y;
                Vector3 offsetOrigin = rayOrigin + offset;
                offsetOrigin = RoundToDecimal(offsetOrigin);
                
                // Cast the ray
                if (Physics.Raycast(offsetOrigin, Vector3.right, out hit, rayLength, hitLayers))
                {
                    dotRenderer.AddParticleAtPoint(hit.point, hit.normal);
                }
                // Cast the ray
                if (Physics.Raycast(offsetOrigin, Vector3.left, out hit, rayLength, hitLayers))
                {
                    dotRenderer.AddParticleAtPoint(hit.point, hit.normal);
                }
                // Cast the ray
                if (Physics.Raycast(offsetOrigin, Vector3.forward, out hit, rayLength, hitLayers))
                {
                    dotRenderer.AddParticleAtPoint(hit.point, hit.normal);
                }
                // Cast the ray
                if (Physics.Raycast(offsetOrigin, Vector3.back, out hit, rayLength, hitLayers))
                {
                    dotRenderer.AddParticleAtPoint(hit.point, hit.normal);
                }
                // Cast the ray
                if (Physics.Raycast(offsetOrigin, Vector3.up, out hit, rayLength, hitLayers))
                {
                    dotRenderer.AddParticleAtPoint(hit.point, hit.normal);
                }
                // Cast the ray
                if (Physics.Raycast(offsetOrigin, Vector3.down, out hit, rayLength, hitLayers))
                {
                    dotRenderer.AddParticleAtPoint(hit.point, hit.normal);
                }
            }
        }
    }
    
    private void CastRaysWorldGrid() {
        var direction = transform.forward;
        var gridResolution = 0.2f;
        Vector3 rayOrigin = transform.position + direction * currentRadius;

        // Normalize the direction to ensure consistent behavior
        direction = direction.normalized;
    
        // Find perpendicular vectors to create the grid plane
        Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
        if (right.magnitude < 0.001f)
        {
            // If direction is parallel to up, use a different reference vector
            right = Vector3.Cross(direction, Vector3.forward).normalized;
        }
        Vector3 up = Vector3.Cross(right, direction).normalized;
    
        // Calculate grid dimensions
        int gridSize = Mathf.CeilToInt(currentRadius * 2 / gridResolution);
    
        // Cast center ray first
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, direction, out hit, rayLength, hitLayers))
        {
            dotRenderer.AddParticleAtPoint(hit.point, hit.normal);
        }
    
        // Cast grid rays
        for (float x = -currentRadius; x <= currentRadius; x += gridResolution)
        {
            for (float y = -currentRadius; y <= currentRadius; y += gridResolution)
            {
                // Skip the center ray as we already processed it
                if (x == 0 && y == 0) continue;
            
                // Calculate offset from center ray
                Vector3 offset = right * x + up * y;
                Vector3 offsetOrigin = rayOrigin + offset;
            
                // Cast the ray
                if (Physics.Raycast(offsetOrigin, direction, out hit, rayLength, hitLayers))
                {
                    dotRenderer.AddParticleAtPoint(hit.point, hit.normal);
                }
            }
        }
    }

    Vector3 RoundToDecimal(Vector3 input) {
        return new Vector3(RoundToDecimal(input.x), RoundToDecimal(input.y), RoundToDecimal(input.z));
    }
    float RoundToDecimal(float input) {
        return Mathf.RoundToInt(input * 2) / 2f;
    }


    private void EmitParticleAtPoint(Vector3 position, Vector3 normal)
    {
        // Set up emission parameters
        emitParams.position = position;
        
        // Don't set rotation - let the particle system handle facing
        // This will use the particle system's default renderer settings
        // For billboarding (facing camera), make sure your particle system has:
        // Renderer module → Render Mode set to "Billboard"
        
        // Optional: If you want to keep surface normal influence for physics
        // but still have billboarding visuals, you can just set velocity
        //emitParams.velocity = normal * 0.1f;
        
        // You can customize more parameters here if needed
        // emitParams.startColor = Color.white;
        emitParams.startSize = Random.Range(0.08f,0.12f);
        // emitParams.startLifetime = 1.0f;
        
        // Emit a single particle with these parameters
        particleSystem.Emit(emitParams, 1);
    }
    
    private void ResetSphere()
    {
        dotRenderer.ClearDots();
        particleSystem.Clear();
        currentRadius = minSphereRadius;
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualize current sphere in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentRadius);
        
        // Visualize max sphere
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, maxSphereRadius);
    }

    private float timer = 0;
    public bool Interact(InteractInput interactInput) {
        if (interactInput.primaryDown) {
            ResetSphere();
            return true;
        }

        if (interactInput.secondaryDown) {
            dotRenderer.ClearAllDots();
            return true;
        }

        /*if (timer <= 0) {
            if (interactInput.scrollDelta > 0) {
                mode += 1;
                mode %= modeCount;
                timer = 0.2f;
            }
            
            if (interactInput.scrollDelta < 0) {
                mode -= 1;
                if (mode < 0) {
                    mode = modeCount-1;
                }
                //mode %= 4;
                timer = 0.2f;
            }
        }

        timer -= Time.deltaTime;*/
        
        
        return false;
    }

    public bool HoldAttention() {
        return false;
    }

    public void DisableTool() {
        // nothing
    }
}
