using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;

namespace Environment
{
    public class Crafting : MonoBehaviour,IInteractable
    {
        public CraftingRecipes recipes;
        public GameObject inventoryParent;
        public GameObject inventoryItemPrefab;
        public Transform recipeParent;
        private List<InventoryItem> _itemsInCrafting;
        public InventoryItem resultSlot;
        private Recipe _activeRecipe;
        
        private void Start()
        {
            _itemsInCrafting = new List<InventoryItem>();
            transform.GetChild(0).gameObject.SetActive(false);
            resultSlot.OnLeftClick += TakeResult;
            resultSlot.OnRightClick += TakeResult;
        }

        private void TakeResult()
        {
            if(resultSlot.Item == null) return;
            resultSlot.HideTooltip();
            Inventory.Instance.AddItem(_activeRecipe.result);
            resultSlot.Item = null;
            var needToRemove = new List<InventoryItem>(_itemsInCrafting.Count);
            foreach (var item in _itemsInCrafting)
            {
                item.Count -= _activeRecipe.ingredients.Find(e => e.item == item.Item.itemName).count;
                if (item.Count <= 0)
                {
                    needToRemove.Add(item);
                }
            }

            foreach (var item in needToRemove)
            {
                Destroy(item.gameObject);
                _itemsInCrafting.Remove(item);
            }
            
            CheckRecipe();
            ShowInventory();
        }

        private void CheckRecipe()
        {
            foreach (var recipe in recipes.recipes)
            {
                bool isValid = true;
                foreach (var ingredient in recipe.ingredients)
                {
                    if (_itemsInCrafting.Any(e => e.Item.itemName == ingredient.item))
                    {
                        if (_itemsInCrafting.Find(e => e.Item.itemName == ingredient.item).Count < ingredient.count)
                        {
                            isValid = false;
                            break;
                        }
                    }
                    else
                    {
                        isValid = false;
                        break;
                    }
                }
                if(!isValid) continue;
                resultSlot.Item = recipe.result;
                _activeRecipe = recipe;
                return;
            }

            _activeRecipe = null;
            resultSlot.Item = null;
        }
        
        private void AddToCrafting(InventoryItem item)
        {
            if (_itemsInCrafting.Any(e=> e.Item == item.Item))
            {
                var itemReference =  _itemsInCrafting.Find(e => e.Item == item.Item);
                itemReference.Count++;
            }
            else
            {
                var newItem = Instantiate(inventoryItemPrefab, recipeParent).GetComponent<InventoryItem>();
                newItem.Item = item.Item;
                newItem.Count = 1;
                _itemsInCrafting.Add(newItem);
                newItem.OnRightClick += () => RemoveFromCrafting(newItem);
            }
            Inventory.Instance.RemoveOne(item.Item.itemName);
            ShowInventory();
            CheckRecipe();
        }

        private void RemoveFromCrafting(InventoryItem item)
        {
            Inventory.Instance.AddItem(item.Item);
            item.Count--;
            if (item.Count == 0)
            {
                Destroy(item.gameObject);
                _itemsInCrafting.Remove(item);
            }    
            ShowInventory();
            CheckRecipe();
        }

        public void SetInteracting()
        {
            
        }

        public void Interact()
        {
            transform.GetChild(0).gameObject.SetActive(true);
            ShowInventory();

            InputHandler.DisableInput = true;
        }

        private void ShowInventory()
        {
            for (int i = 0; i < inventoryParent.transform.childCount; i++)
            {
                Destroy(inventoryParent.transform.GetChild(i).gameObject);
            }

            foreach (var item in Inventory.Instance.RenderInventory())
            {
                item.transform.SetParent(inventoryParent.transform);
                item.OnLeftClick += () => AddToCrafting(item);
            }
        }

        public void Quit()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            foreach (var t in _itemsInCrafting)
            {
                Destroy(t.gameObject);
                Inventory.Instance.ThrowItem(t);
            }

            _itemsInCrafting.Clear();
            InputHandler.DisableInput = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                IsInteractable = true;
                PlayerMovement._interactable = this;
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

        private bool IsInteractable { get; set; }
    }
}