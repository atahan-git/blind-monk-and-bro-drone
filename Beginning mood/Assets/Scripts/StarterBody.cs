using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterBody : MonoBehaviour {
	public ControllableBody body;
    public PlayerController playerController;
    
    void Start() {
	    playerController.AssumeBody(body.body);
    }
}
