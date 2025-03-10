using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DotsMaker : MonoBehaviour {

    public GameObject particlesSurface;
    public GameObject particlesDistant;

    private void Start() {
        MakeDots();
    }

    [ContextMenu("make particles")]
    public void MakeDots() {
        transform.DeleteAllChildrenEditor();
        
        var renderers = FindObjectsOfType<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++) {
            if (renderers[i].gameObject.layer == 11 || renderers[i].gameObject.layer == 12) {
                continue;
            }

            if (renderers[i].GetComponent<TMP_Text>()) {
                continue;
            }

            const int amount = 10;
            
            var surface = Instantiate(particlesSurface, transform).GetComponent<ParticleSystem>();
            var distant = Instantiate(particlesDistant, transform).GetComponent<ParticleSystem>();
            surface.gameObject.SetActive(true);
            distant.gameObject.SetActive(true);

            var meshArea = CalculateSurfaceArea(renderers[i].GetComponent<MeshFilter>().sharedMesh);
            var surfaceShape = surface.shape;
            surfaceShape.meshRenderer = renderers[i];
            var surfaceEmission = surface.emission;
            surfaceEmission.rateOverTime = meshArea * amount*5;
            
            var distantShape = distant.shape;
            distantShape.meshRenderer = renderers[i];
            var distantEmission = distant.emission;
            distantEmission.rateOverTime = meshArea * amount;


            var dissapearingPlatform = renderers[i].GetComponent<DisappearingPlatform>();
            if (dissapearingPlatform) {
                dissapearingPlatform.myParticles[0] = surface;
                dissapearingPlatform.myParticles[1] = distant;
            }
        }
    }
    
    float CalculateSurfaceArea(Mesh mesh) {
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;

        double sum = 0.0;

        for(int i = 0; i < triangles.Length; i += 3) {
            Vector3 corner = vertices[triangles[i]];
            Vector3 a = vertices[triangles[i + 1]] - corner;
            Vector3 b = vertices[triangles[i + 2]] - corner;

            sum += Vector3.Cross(a, b).magnitude;
        }

        return (float)(sum/2.0);
    }
}
