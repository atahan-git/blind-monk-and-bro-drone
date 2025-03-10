using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LazerSensor : MonoBehaviour
{
    public UnityEvent OnPowered = new UnityEvent();
    public UnityEvent OnLosePower = new UnityEvent();

    public float power = 0;
    private bool curPowerState = false;
    void Update() {
        var detectCorrect = power > 0;

        if (curPowerState != detectCorrect) {
            curPowerState = detectCorrect;
            if (curPowerState) {
                OnPowered?.Invoke();
            } else {
                OnLosePower?.Invoke();
            }
        }
    }
}
