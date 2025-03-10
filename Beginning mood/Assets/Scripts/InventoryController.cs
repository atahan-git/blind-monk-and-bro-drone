using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {

    public GameObject[] alwaysAttentionTools;
    public GameObject[] alwaysActiveTools;
    public GameObject[] swappableTools;

    public int startTool = 1;
    
    void Awake() {
        SetToSwappableTool(startTool);
    }


    private IPlayerTool lastAttentionTool;
    public void Interact(InteractInput interactInput) {

        for (int i = 0; i < alwaysAttentionTools.Length; i++) {
            alwaysAttentionTools[i].GetComponent<IPlayerTool>().Interact(interactInput);
        }
        

        var toolGrabbedAttention = false;
        if (lastAttentionTool != null) {
            if (lastAttentionTool.HoldAttention()) {
                toolGrabbedAttention = lastAttentionTool.Interact(interactInput);
                if (toolGrabbedAttention) {
                    return;
                } else {
                    lastAttentionTool = null;
                }
            }
        }
        
        for (int i = 0; i < alwaysActiveTools.Length; i++) {
            var tool = alwaysActiveTools[i].GetComponent<IPlayerTool>();
            toolGrabbedAttention = tool.Interact(interactInput);
            if (toolGrabbedAttention) {
                if (lastAttentionTool != tool) {
                    lastAttentionTool?.DisableTool();
                    lastAttentionTool = tool;
                }
                return;
            }
        }

        if (currentTool != null) {
            toolGrabbedAttention = currentTool.Interact(interactInput);
            if (toolGrabbedAttention) {
                if (lastAttentionTool != currentTool) {
                    lastAttentionTool?.DisableTool();
                    lastAttentionTool = currentTool;
                }
                return;
            }
        }

        if (interactInput.numInput >= 0) {
            SetToSwappableTool(interactInput.numInput);
        }
    }

    public IPlayerTool currentTool;
    
    void SetToSwappableTool(int index) {
        index -= 1;
        for (int i = 0; i < swappableTools.Length; i++) {
            swappableTools[i].GetComponent<IPlayerTool>().DisableTool();
            swappableTools[i].SetActive(false);
        }

        if (index >= 0 && index < swappableTools.Length) {
            swappableTools[index].SetActive(true);
            currentTool = swappableTools[index].GetComponent<IPlayerTool>();
        }
    }

    public void DisableAllTools() {
        for (int i = 0; i < alwaysActiveTools.Length; i++) {
            var tool = alwaysActiveTools[i].GetComponent<IPlayerTool>();
            tool.DisableTool();
        }

        if (currentTool != null) {
            currentTool.DisableTool();
        }
    }
}

public interface IPlayerTool {
    public bool Interact(InteractInput interactInput);

    public bool HoldAttention();
    public void DisableTool();
}
