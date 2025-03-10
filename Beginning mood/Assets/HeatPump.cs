using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatPump : MonoBehaviour {
    public List<Heatable> heatPumpIn = new List<Heatable>();
    public List<Heatable> heatPumpOut = new List<Heatable>();

    private float timer;

    public void NumbersChanged() {
        timer = 0.5f;
    }

    private int offset = 0;
    void Update()
    {
        if (timer <= 0) {
            var heatIn = 0;

            for (int i = 0; i < heatPumpIn.Count; i++) {
                heatIn += 1;
                heatPumpIn[i].ChangeHeat(-1);
            }


            while (heatIn > 0) {
                for (int i = 0; i < heatPumpOut.Count; i++) {
                    var index = (i + offset)% heatPumpOut.Count;
                    heatPumpOut[index].ChangeHeat(1);
                    heatIn -= 1;
                    if (heatIn <= 0) {
                        break;
                    }
                }

                offset += 1;
                offset = offset % heatPumpOut.Count;
            }

            timer = 0.5f;
        }

        timer -= Time.deltaTime;
    }
}
