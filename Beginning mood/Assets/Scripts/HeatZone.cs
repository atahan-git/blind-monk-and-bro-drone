using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeatZone : MonoBehaviour {
    public List<Heatable> heatables = new List<Heatable>();

    public int temperature = 2;
    
    private void Start() {
        BoxCollider box = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.attachedRigidbody != null) {
            var heatable = other.attachedRigidbody.GetComponent<Heatable>();

            if (heatable != null) {
                heatables.Add(heatable);
            }
        }
    }

    
    public TMP_Text[] texts;
    private void Update() {
        for (int i = 0; i < texts.Length; i++) {
            texts[i].text = (temperature*10).ToString();
        }

        /*for (int i = 0; i < heatables.Count; i++) {
            if (heatables[i].temperature > temperature) {
                if (!coolingDown.Contains(heatables[i])) {
                    coolingDown.Add(heatables[i]);
                    StartCoroutine(CoolDown(heatables[i]));
                }
            }
        }*/
    }

    public List<Heatable> coolingDown = new List<Heatable>();
    IEnumerator CoolDown(Heatable heatable) {
        yield return new WaitForSeconds(10);
        
        while (true) {
            if (heatable.temperature <= temperature) {
                coolingDown.Remove(heatable);
                yield break;
            }

            heatable.temperature -= 1;
            
            yield return new WaitForSeconds(10);
        }
    }
}
