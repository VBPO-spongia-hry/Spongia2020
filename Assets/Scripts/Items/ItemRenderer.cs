using System;
using UnityEngine;

namespace Items
{
    public class ItemRenderer : MonoBehaviour, IInteractable
    {
        public bool IsInteractable { get; set; }
        public Item item;

        private void Start()
        {
            IsInteractable = false;
            GetComponent<SpriteRenderer>().sprite = item.icon;
        }

        public void SetInteracting()
        {
            PlayerMovement._interactable = this;
        }

        public void Interact()
        {
            Inventory.Instance.AddItem(item);
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                IsInteractable = Inventory.Instance.CanAddItem(item);
                PlayerMovement._interactable = IsInteractable ? this : null;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                IsInteractable = false;
                PlayerMovement._interactable = null;
            }
        }

        private void OnMouseEnter()
        {
            if (IsInteractable)
            {
                Inventory.Instance.ShowTooltip(item);
            }
        }

        private void OnMouseExit()
        {
            Inventory.Instance.HideTooltip();
        }
    }
}