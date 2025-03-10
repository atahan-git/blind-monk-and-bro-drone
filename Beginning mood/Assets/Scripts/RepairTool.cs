using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;
using UnityEngine.UI;

public class RepairTool : MonoBehaviour, IPlayerTool
{
    
    public ObjectSelectionHelper<RepairableBurnEffect> selector = new ObjectSelectionHelper<RepairableBurnEffect>();

    
    public Image validRepairImage;
    public Slider repairingSlider;
    
    public float curRepairTime;
    public LayerMask repairLookMask;
    public LayerMask blockerLookMask;
    public bool repairing = false;
    
    public HighlightProfile highlightProfile;
    public HighlightProfile selectProfile;

    public ParticleSystem repairParticles;
    private bool doRepair;
    private AudioPlayer _audioPlayer;

    private void Start() {
        _audioPlayer = GetComponentInChildren<AudioPlayer>();
    }

    public bool Interact(InteractInput interactInput) {
        var camTrans = PlayerController.mainCam.transform;
        var camPos = camTrans.position;
        Ray ray = new Ray(camPos, camTrans.forward);
        RaycastHit hit;
        var validTarget = false;
    
        var allCast = Physics.SphereCastAll(ray, 0.5f, 5, repairLookMask);
        RepairableBurnEffect closestRegular = null;
        float regularMinDist = float.MaxValue;
    
        for (int i = 0; i < allCast.Length; i++) {
            var target = allCast[i].collider.GetComponentInParent<RepairableBurnEffect>();
    
            if (target != null) {
                /*if(!CanSeeTarget(target, camPos))
                    continue;*/
    					
                var dist = Vector3.Distance(camPos, target.transform.position);
                if (dist < regularMinDist) {
                    closestRegular = target;
                    regularMinDist = dist;
                }
            }
        }
    
        if (closestRegular != null) {
            selector.Select(closestRegular, selectProfile);
        } else {
            selector.Deselect();
        }
    			
        if (selector.curObject != null) {
            validTarget = true;
        }
    
        validRepairImage.gameObject.SetActive(validTarget);
        
        if (interactInput.primaryDown) {
            doRepair = true;
        }

        if (interactInput.primaryUp) {
            doRepair = false;
        }

        var isRepairing = doRepair && validTarget;
        if (isRepairing) {
            repairParticles.Play();
            _audioPlayer.Play();
        } else {
            repairParticles.Stop();
            _audioPlayer.Stop();
        }
        
        repairing = false;
        if (validTarget) {
            if (doRepair) {
                repairing = true;
                selector.Select(selector.curObject, highlightProfile);
                
                curRepairTime += 0.5f * Time.deltaTime;
                if (curRepairTime >= 1) {
                    curRepairTime = 0;
                    Destroy(selector.curObject.gameObject);
                }
            } else {
                curRepairTime -= Time.deltaTime/2f;
            }
        } else {
            curRepairTime -= Time.deltaTime;
        }

        curRepairTime = Mathf.Clamp(curRepairTime, 0, 1);
    
        repairingSlider.value = curRepairTime;

        return repairing;
    }
    
    
    bool CanSeeTarget(RepairableBurnEffect effect, Vector3 camPos) {
        if (Physics.Raycast(camPos, effect.transform.position - camPos, out RaycastHit hit, 10, blockerLookMask)) {
            var depth = Vector3.Distance(hit.point, effect.transform.position);
    
            if (depth > 0.1f) {
                return false;
            } else {
                return true;
            }
    			
        } else {
            return true;
        }
    }

    public bool HoldAttention() {
        return repairing;
    }

    public void DisableTool() {
        repairingSlider.value = 0;
        validRepairImage.gameObject.SetActive(false);
    
        curRepairTime = 0;
        doRepair = false;
    }
}
