using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialMaster : MonoBehaviour {

    /*public static MaterialMaster s;

    private void Awake() {
        s = this;
    }*/

    public MaterialSet[] materialSets;


    public MaterialSet GetSet(MaterialType type) {
        for (int i = 0; i < materialSets.Length; i++) {
            if (materialSets[i].myType == type) {
                return materialSets[i];
            }
        }

        return materialSets[0];
    }


    [ContextMenu("SetMaterials")]
    public void EditorSetAllMaterials() {
        var myObjects = FindObjectsOfType<MaterialSetter>();

        for (int i = 0; i < myObjects.Length; i++) {
            var rend = myObjects[i].GetComponent<MeshRenderer>();
            var set = GetSet(myObjects[i].myType);
            rend.sharedMaterials = new[] { /*set.blindsightMat,*/ set.dronesightMat };
        }
        
        print($"Swapped {myObjects.Length} materials");
    }
}

public enum MaterialType {
    Grid=0, Atlas=1, Regular=2, Zinc=3, Gold=4, BlindsightGlass=5, DronesightGlass=6, BlindsightPlatform=7, DronesightPlatform=8
}

[Serializable]
public class MaterialSet {
    public MaterialType myType;
    public Material blindsightMat;
    public Material dronesightMat;
}
