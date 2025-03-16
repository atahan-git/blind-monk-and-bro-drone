using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychEnergyCable : MonoBehaviour {
    public bool isOn = false;

    public Material[] offMaterials;
    public Material[] onMaterials;

    
    public int cableId = -1;

    public static List<PsychEnergyCable> cables = new List<PsychEnergyCable>();

    private void Awake() {
        cables.Add(this);
    }

    private void OnDestroy() {
        cables.Remove(this);
    }

    public void ApplyState() {
        var renderer = GetComponent<MeshRenderer>();

        renderer.sharedMaterials = isOn ? onMaterials : offMaterials;
    }
    // Start is called before the first frame update
    void Start()
    {
        ApplyState();
    }
}
