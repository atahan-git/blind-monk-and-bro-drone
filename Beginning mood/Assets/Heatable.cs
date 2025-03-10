using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Heatable : MonoBehaviour {

    
    public int temperature = 0;

    private Material heatMat;
    
    public List<Heatable> contacts = new List<Heatable>();
    public bool modifyHeatShader = true;
    private void Start() {
        if (modifyHeatShader) {
            heatMat = GetComponent<Renderer>().materials[1];
        }

        ChangeHeat(0);
    }

    public TMP_Text[] texts;
    private static readonly int K = Shader.PropertyToID("_temperature");

    const int MaxHeat = 5;
    const int MinHeat = -3;

    void Update() {
        for (int i = 0; i < texts.Length; i++) {
            texts[i].text = (temperature * 10).ToString();
        }

        if (modifyHeatShader) {
            heatMat.SetFloat(K, temperature);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.attachedRigidbody != null) {
            var heatable = other.attachedRigidbody.GetComponent<Heatable>();

            if (heatable != null) {
                if (!contacts.Contains(heatable)) {
                    contacts.Add(heatable);
                    if (!heatable.contacts.Contains(this)) {
                        StartCoroutine(HeatTransferLoop(heatable));
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.attachedRigidbody != null) {
            var heatable = other.attachedRigidbody.GetComponent<Heatable>();

            if (heatable != null) {
                contacts.Remove(heatable);
            }
        }
    }


    public bool keepTemperature = false;
    public virtual void ChangeHeat(int amount) {
        if (!keepTemperature) {
            temperature += amount;
        }
        temperature = Mathf.Clamp(temperature, MinHeat, MaxHeat);
    }

    public List<Heatable> myTransferTargets = new List<Heatable>();
    IEnumerator HeatTransferLoop(Heatable contact) {
        myTransferTargets.Add(contact);
        yield return new WaitForSeconds(0.5f);

        while (contacts.Contains(contact)) {
            var tempDif = temperature - contact.temperature;

            if (tempDif > 0) {
                ChangeHeat(-1);
                contact.ChangeHeat(1);
            } else if (tempDif < 0) {
                ChangeHeat(1);
                contact.ChangeHeat(-1);
            }

            if (Mathf.Abs(temperature - contact.temperature) == 0) {
                while (Mathf.Abs(temperature - contact.temperature) == 0) {
                    yield return null;
                }
                yield return new WaitForSeconds(0.5f);
            } else {
                yield return new WaitForSeconds(0.5f);
            }
        }

        myTransferTargets.Remove(contact);
    }
}
