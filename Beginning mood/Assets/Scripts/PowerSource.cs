using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerSource : MonoBehaviour {

    public UnityEvent OnPowered = new UnityEvent();
    public UnityEvent OnLosePower = new UnityEvent();

    public PowerCable[] cables;

    public bool isPowered = false;

    void Update() {
        var cablesConnect = true;
        for (int i = 0; i < cables.Length; i++) {
            cablesConnect = cablesConnect && cables[i].IsWorking();
        }

        if (!isPowered && cablesConnect) {
            isPowered = true;
            OnPowered?.Invoke();
        }else if (isPowered && !cablesConnect) {
            isPowered = false;
            OnLosePower?.Invoke();
        }
    }
}


public abstract class PowerCable : MonoBehaviour {
    public abstract bool IsWorking();
}