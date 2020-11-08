using System;
using Missions;
using UnityEngine;

namespace Environment
{
    public class PowerSwitch : MonoBehaviour,IInteractable
    {
        [NonSerialized] public static Mission Mission;
        
        public void SetInteracting()
        {
            
        }

        private void Start()
        {
            return;
        }

        public void Interact()
        {
            Mission.UpdateProgress();
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