using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsyhEnergyBase : MonoBehaviour
{
    public bool isOn = false;

    public Material[] offMaterials;
    public Material[] onMaterials;

    public GameObject onCyclinder;

    public Transform cubeSnapPos;

    public int cableId = -1;
    public void ApplyState() {
        var renderer = GetComponentInChildren<MeshRenderer>();

        renderer.sharedMaterials = isOn ? onMaterials : offMaterials;
        onCyclinder.SetActive(isOn);

        for (int i = 0; i < PsychEnergyCable.cables.Count; i++) {
            var cable = PsychEnergyCable.cables[i];
            if (cable.cableId == cableId) {
                cable.isOn = isOn;
                cable.ApplyState();
            }
        }
        
        for (int i = 0; i < PsychDoorLight.cables.Count; i++) {
            var cable = PsychDoorLight.cables[i];
            if (cable.cableId == cableId) {
                cable.isOn = isOn;
                cable.ApplyState();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        ApplyState();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<EnergyCube>() is { } energyCube) {
            energyCube.GetComponent<Carryable>().DestroySelf();
            Destroy(energyCube.GetComponent<Rigidbody>());
            energyCube.transform.position = cubeSnapPos.position;
            energyCube.transform.rotation = cubeSnapPos.rotation;
            isOn = true;
            ApplyState();
        }
    }
}
