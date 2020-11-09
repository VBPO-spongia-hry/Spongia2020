using System;
using Missions;
using UnityEngine;

namespace Environment
{
    public class PowerSwitch : MonoBehaviour,IInteractable
    {
        [NonSerialized] public static Mission Mission;
        public Sprite on;
        public Sprite off;
        
        public void SetInteracting()
        {
            
        }

        private void Start()
        {
            return;
        }

        private void Update()
        {
            if (GetComponentInParent<MapLocation>().hasPower)
            {
                GetComponent<SpriteRenderer>().sprite = on;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = off;
            }
        }

        public void Interact()
        {
            if(Mission != null) Mission.UpdateProgress();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Enter trigger");
            if (other.CompareTag("Player"))
            {
                IsInteractable = !Mission.Complete;
                PlayerMovement._interactable = IsInteractable ? this : null;
            }
        }

        private bool IsInteractable { get; set; }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                IsInteractable = false;
                PlayerMovement._interactable = null;
            }
        }
    }
}