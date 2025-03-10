using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Carryable : MonoBehaviour {
	[HideInInspector]
	public float drag;
	[HideInInspector]
	public float angularDrag;

	private void Start() {
		var rg = GetComponent<Rigidbody>();
		drag = rg.drag;
		angularDrag = rg.angularDrag;
	}
}
