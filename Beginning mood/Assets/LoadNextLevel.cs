using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour {
    public SceneReference nextScene;

    private bool isLoading = false;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (!isLoading) {
                SceneManager.LoadSceneAsync(nextScene);
                isLoading = true;
            }
        }
    }
}
