using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetLine : MonoBehaviour {
    private GameObject one;
    public GameObject two;
    private LineRenderer _lineRenderer;
    private bool isActivated = false;
    public void SetTargets(GameObject _one, GameObject _two) {
        one = _one;
        two = _two;
        _lineRenderer = GetComponentInChildren<LineRenderer>(true);
        _lineRenderer.gameObject.SetActive(true);
        isActivated = true;
    }
    // Update is called once per frame
    void Update() {
        if (isActivated) {
            _lineRenderer.SetPosition(0, one.transform.position);
            _lineRenderer.SetPosition(1, two.transform.position);
        }
    }
}
