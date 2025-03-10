using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorControl : MonoBehaviour
{
     public bool isOpen;
    
        public GameObject door;
    
        private Vector3 closePos;

        void Start() {
            closePos = door.transform.position;
        }
    
        // Update is called once per frame
        void Update() {
            var targetPos = isOpen ? closePos + Vector3.down * 43.2f : closePos;
            
            door.transform.position = Vector3.MoveTowards(door.transform.position, targetPos, 1.5f*Time.deltaTime);
        }
    
        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                if (!isOpen) {
                    SoundController.s.DoFlip();
                }
                
                other.gameObject.transform.SetParent(door.transform);

                isOpen = true;
            }
        }
}
