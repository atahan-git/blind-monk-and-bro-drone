#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

#ifndef MY_CUSTOM_LIGHTING
#define MY_CUSTOM_LIGHTING
// This file contains functions for calculating lighting in ShaderGraph
// Based on URP lighting model but with custom modifications

#if !SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif


// Main light calculation with shadows
void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
    #if defined(SHADERGRAPH_PREVIEW)
        // In preview, we just use some default values
        Direction = float3(0.5, 0.5, 0);
        Color = float3(1, 1, 1);
        DistanceAtten = 1;
        ShadowAtten = 1;
    #else
        // Get the main light
        #if SHADOWS_SCREEN
            half4 clipPos = TransformWorldToHClip(WorldPos);
            half4 shadowCoord = ComputeScreenPos(clipPos);
        #else
            half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        #endif
        
        Light mainLight = GetMainLight(shadowCoord);
        
        Direction = mainLight.direction;
        Color = mainLight.color;
        DistanceAtten = mainLight.distanceAttenuation;
        ShadowAtten = mainLight.shadowAttenuation;
    #endif
}

// Calculate additional lights contribution
void AdditionalLights_float(float3 WorldPosition, float3 WorldNormal, float3 WorldView, 
                           float MainDiffuse, float MainSpecular, float Smoothness,
                           out float3 Diffuse, out float3 Specular)
{
    float3 diffuseColor = float3(0, 0, 0);
    float3 specularColor = float3(0, 0, 0);

    #ifndef SHADERGRAPH_PREVIEW
        // Calculate additional lights only outside of preview
        int pixelLightCount = GetAdditionalLightsCount();
        
        // Smoothness to shininess conversion (pow(smoothness, 2) * 128)
        float shininess = exp2(10 * Smoothness + 1);
        
        for (int i = 0; i < pixelLightCount; ++i)
        {
            Light light = GetAdditionalLight(i, WorldPosition);
            
            // Diffuse calculation
            float NdotL = saturate(dot(WorldNormal, light.direction));
            float lightIntensity = NdotL * light.distanceAttenuation * light.shadowAttenuation;
            diffuseColor += light.color * lightIntensity;
            
            // Specular calculation (Blinn-Phong)
            float3 halfVector = normalize(light.direction + WorldView);
            float NdotH = saturate(dot(WorldNormal, halfVector));
            float specularIntensity = pow(NdotH, shininess) * lightIntensity * Smoothness;
            specularColor += light.color * specularIntensity;
        }
    #endif

    // Output diffuse and specular contributions from additional lights
    Diffuse = diffuseColor;
    Specular = specularColor;
}

// Combined lighting calculation
void CalculateCustomLighting_float(float3 Position, float3 Normal, float3 ViewDirection, 
                               float3 BaseColor, float Metallic, float Smoothness, 
                               out float3 FinalColor)
{
    // Get main light info
    float3 mainLightDir, mainLightColor;
    float mainLightDistanceAtten, mainLightShadowAtten;
    MainLight_float(Position, mainLightDir, mainLightColor, mainLightDistanceAtten, mainLightShadowAtten);
    
    // Calculate ambient color (simplified)
    float3 ambientColor = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w) * 0.5;
    
    // Calculate main light diffuse
    float NdotL = saturate(dot(Normal, mainLightDir));
    float3 diffuseColor = BaseColor * mainLightColor * NdotL * mainLightDistanceAtten * mainLightShadowAtten;
    
    // Calculate main light specular (Blinn-Phong)
    float3 halfVector = normalize(mainLightDir + ViewDirection);
    float NdotH = saturate(dot(Normal, halfVector));
    float specularPower = exp2(10 * Smoothness + 1);
    float3 specularColor = mainLightColor * pow(NdotH, specularPower) * Smoothness * mainLightDistanceAtten * mainLightShadowAtten;
    
    // Add contribution from additional lights
    float3 additionalDiffuse, additionalSpecular;
    AdditionalLights_float(Position, Normal, ViewDirection, NdotL, NdotH, Smoothness, additionalDiffuse, additionalSpecular);
    
    // Combine all lighting contributions
    float oneMinusMetallic = 1.0 - Metallic;
    float3 finalDiffuse = (ambientColor + diffuseColor + additionalDiffuse) * BaseColor * oneMinusMetallic;
    float3 finalSpecular = (specularColor + additionalSpecular) * lerp(0.04, BaseColor, Metallic);
    
    FinalColor = finalDiffuse + finalSpecular;
}

// Helper function for shadow casting
void GetShadowCoordinates_float(float3 WorldPos, out float4 ShadowCoord)
{
    #if defined(SHADERGRAPH_PREVIEW)
        ShadowCoord = float4(0, 0, 0, 0);
    #else
        // Compute shadow coordinates in screen space or shadow map space
        #if SHADOWS_SCREEN
            float4 clipPos = TransformWorldToHClip(WorldPos);
            ShadowCoord = ComputeScreenPos(clipPos);
        #else
            ShadowCoord = TransformWorldToShadowCoord(WorldPos);
        #endif
    #endif
}

// Shadow map sampling function
void SampleShadowMap_float(float4 ShadowCoord, out float ShadowAtten)
{
    #if defined(SHADERGRAPH_PREVIEW)
        ShadowAtten = 1.0;
    #else
        #if SHADOWS_SCREEN
            ShadowAtten = SampleScreenSpaceShadowmap(ShadowCoord);
        #else
            ShadowAtten = SampleShadowmap(ShadowCoord);
        #endif
    #endif
}

// Custom function for more control over shadow softness
void CustomShadowAttenuation_float(float3 WorldPos, float SoftnessFactor, out float ShadowAtten)
{
    #if defined(SHADERGRAPH_PREVIEW)
        ShadowAtten = 1.0;
    #else
        float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        
        // Get raw shadow attenuation
        float attenuation = MainLightRealtimeShadow(shadowCoord);
        
        // Apply custom shadow softness
        if (SoftnessFactor > 0)
        {
            // Sample multiple shadow map taps for softer shadows
            float2 shadowMapSize = _MainLightShadowmapSize.xy;
            float2 texelSize = 1.0 / shadowMapSize;
            float2 offset = texelSize * SoftnessFactor;
            
            // Sample additional points around the main sample
            float shadow = attenuation;
            shadow += MainLightRealtimeShadow(shadowCoord + float4(offset.x, 0, 0, 0));
            shadow += MainLightRealtimeShadow(shadowCoord - float4(offset.x, 0, 0, 0));
            shadow += MainLightRealtimeShadow(shadowCoord + float4(0, offset.y, 0, 0));
            shadow += MainLightRealtimeShadow(shadowCoord - float4(0, offset.y, 0, 0));
            
            // Average the samples
            ShadowAtten = shadow * 0.2;
        }
        else
        {
            ShadowAtten = attenuation;
        }
    #endif
}

#endif
#endif // CUSTOM_LIGHTING_INCLUDED