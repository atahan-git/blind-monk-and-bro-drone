using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls alpha clipping for all materials using a specified shader in the scene.
/// </summary>
public class AlphaClippingController : MonoBehaviour
{
    [Tooltip("The name of the shader to target for alpha clipping control")]
    public string targetShaderName = "Your Shader Name";
    
    [Tooltip("The property name for alpha clipping in your shader (often '_Cutoff' or '_AlphaClip')")]
    public string alphaClipPropertyName = "_AlphaClip";
    
    [Tooltip("The property name for enabling/disabling alpha clipping (often '_AlphaClipThreshold' or '_Cutoff')")]
    public string alphaClipThresholdPropertyName = "_Cutoff";
    
    [Tooltip("Default threshold value when alpha clipping is enabled")]
    public float defaultClipThreshold = 0.5f;
    
    // Cached list of materials using the target shader
    private List<Material> targetMaterials = new List<Material>();
    
    // Track current state
    private bool isAlphaClippingEnabled = false;

    public static AlphaClippingController s;

    void Awake() {
        s = this;
    }

    private void Start()
    {
        // Find all materials using the target shader on scene start
        FindTargetMaterials();
        SetAlphaClipping(true);
    }

    private void OnApplicationQuit() {
        EnableEverything();
    }

    /// <summary>
    /// Finds all materials in the scene that use the specified shader.
    /// </summary>
    public void FindTargetMaterials()
    {
        targetMaterials.Clear();
        
        // Get all renderers in the scene
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                /*if (material != null && material.shader != null) {
                    print(material.shader.name);
                }*/

                if (material != null && material.shader != null && 
                    material.shader.name == targetShaderName)
                {
                    // Add unique materials to our list
                    if (!targetMaterials.Contains(material))
                    {
                        targetMaterials.Add(material);
                    }
                }
            }
        }
        
        Debug.Log($"Found {targetMaterials.Count} materials using shader '{targetShaderName}'");
    }

    /// <summary>
    /// Enables alpha clipping on all target materials.
    /// </summary>
    [ContextMenu("Set Monk Mode")]
    public void EnableAlphaClipping()
    {
        
        SetAlphaClipping(true);
    }

    [ContextMenu("Set Regular Edit Mode")]
    public void EnableEverything() {
        
        SetAlphaClipping(false);
        Shader.SetGlobalFloat("_BlindsightAlpha", 1);
        Shader.SetGlobalFloat("_EditorExtraMultiplier", 1000);
    }

    /// <summary>
    /// Disables alpha clipping on all target materials.
    /// </summary>
    [ContextMenu("Set Drone Mode")]
    public void DisableAlphaClipping()
    {
        SetAlphaClipping(false);
    }

    /// <summary>
    /// Toggles the current alpha clipping state.
    /// </summary>
    public void ToggleAlphaClipping()
    {
        SetAlphaClipping(!isAlphaClippingEnabled);
    }

    /// <summary>
    /// Sets the alpha clipping state for all target materials.
    /// </summary>
    /// <param name="enabled">Whether alpha clipping should be enabled or disabled</param>
    public void SetAlphaClipping(bool enabled)
    {
        isAlphaClippingEnabled = enabled;
        
        // Refresh materials list if empty
        if (targetMaterials.Count == 0)
        {
            FindTargetMaterials();
        }

        Shader.SetGlobalFloat("_EditorExtraMultiplier", 1);
        Shader.SetGlobalFloat("_BlindsightAlpha", enabled ? 1 : 0);

        foreach (Material material in targetMaterials)
        {
            // Set the alpha clip property if it exists
            if (material.HasProperty(alphaClipPropertyName)) {
                if (enabled) {
                    //material.EnableKeyword("_ALPHATEST_ON");
                    material.SetFloat("_HologramStrength", 1);
                    //material.SetFloat("_AlphaClip", 0);
                    material.SetInt("_AlwaysVisible", 0);
                } else {
                    //material.DisableKeyword("_ALPHATEST_ON");
                    material.SetFloat("_HologramStrength", 0);
                    material.SetInt("_AlwaysVisible", 1);
                }
            }

            /*// Set the clip threshold to either 0 (disabled) or default value (enabled)
            if (material.HasProperty(alphaClipThresholdPropertyName))
            {
                material.SetFloat(alphaClipThresholdPropertyName, enabled ? defaultClipThreshold : 0.0f);
            }*/
        }
        
        Debug.Log($"Alpha clipping {(enabled ? "enabled" : "disabled")} on {targetMaterials.Count} materials");
    }
}