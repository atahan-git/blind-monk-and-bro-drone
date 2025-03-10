using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableBody : MonoBehaviour {
	public ControllableBodyStruct body;
}

[Serializable]
public class ControllableBodyStruct {
	public GameObject body;
	public GameObject cam;
	public GameObject inventory;
} 
