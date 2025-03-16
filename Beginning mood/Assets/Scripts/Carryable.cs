using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
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

	public void DestroySelf() {
		Destroy(GetComponent<HitSoundSource>());
		Destroy(GetComponent<HighlightEffect>());
		Destroy(GetComponent<Rigidbody>());
		Destroy(this);
	}
}
