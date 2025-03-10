using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatLazer : MonoBehaviour {

    public List<Vector3> locations;

    public float range = 100;
    public LayerMask layerMask;

    public float heatTimer = 0;
    public int heat = 1;

    private LineRenderer _lineRenderer;

    private void Start() {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        locations.Clear();
        locations.Add(transform.position);
        
        ShootLazer(transform.position, transform.forward, 0);

        _lineRenderer.positionCount = locations.Count;
        _lineRenderer.SetPositions(locations.ToArray());
    }

    void ShootLazer(Vector3 position, Vector3 direction, int depth) {
        Debug.DrawLine(position, position + direction*1, Color.green);
        
        Ray ray = new Ray(position, direction);

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, range, layerMask)) {

            locations.Add(hitInfo.point);
            if (hitInfo.collider.GetComponent<Mirror>()) {
                if (depth < 10) {
                    ShootLazer(hitInfo.point, Vector3.Reflect(direction, hitInfo.normal), depth + 1);
                }
            } else if (hitInfo.collider.GetComponent<LazerSensor>()) {
                hitInfo.collider.GetComponent<LazerSensor>().power = 1f;
                heatTimer = 0.5f;
            } else if (hitInfo.collider.attachedRigidbody != null) {
                var heatable = hitInfo.collider.attachedRigidbody.GetComponent<Heatable>();
                if (heatable != null) {
                    heatTimer -= Time.deltaTime;
                    if (heatTimer <= 0) {
                        heatable.ChangeHeat(heat);
                        heatTimer = 0.5f;
                    }
                } else {
                    heatTimer = 0.5f;
                }
            } else {
                heatTimer = 0.5f;
            }

        } else {
            heatTimer = 0.5f;
            locations.Add(transform.position + transform.forward * 10);
        }
    }
}
