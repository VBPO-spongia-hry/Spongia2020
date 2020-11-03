using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Missions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance;
        public GameObject itemsParent;
        public GameObject tooltip;
        private List<InventoryItem> _items;
        public Slider capacitySlider;
        public TextMeshProUGUI capacityText;
        public GameObject powerUpIcon;
        [NonSerialized] public bool CanUsePowerUp = true;
        
        private void Start()
        {
            Usage = 0;
            capacitySlider.maxValue = 100;
            powerUpIcon.SetActive(false);
        }

        public int Capacity => Backpack == null ? 100 : Backpack.backpackCapacity;

        private int _usage;
        private int Usage
        {
            get => _usage;
            set
            {
                _usage = value;
                capacitySlider.value = _usage;
                capacityText.SetText($"{_usage}/{Capacity}");
            }
        }
        public InventoryItem armorSlot;
        public Item Armor
        {
            get => armorSlot.Item;
            set => armorSlot.Item = value;
        }

        public InventoryItem protectionSlot;

        public Item Protection
        {
            get => protectionSlot.Item;
            set => protectionSlot.Item = value;
        }

        public InventoryItem weaponSlot;
        public Item Weapon
        {
            get => weaponSlot.Item;
            set => weaponSlot.Item = value;
        }
        public InventoryItem backpackSlot;
        public Item Backpack
        {
            get => backpackSlot.Item;
            set
            {
                backpackSlot.Item = value;
                capacitySlider.maxValue = Backpack == null ? 100 : backpackSlot.Item.backpackCapacity;
                capacityText.SetText($"{Usage}/{Capacity}");
            } 
        }
        public GameObject itemUIPrefab;
        public GameObject itemPrefab;

        private void OnEnable()
        {
            if(Instance != null) Destroy(Instance);
            Instance = this;
            _items = new List<InventoryItem>();
        }

        public void AddItem(Item item)
        {
            if(!CanAddItem(item)) return;
            if (ContainsItem(item.name))
            {
                var slot = _items.FirstOrDefault(inventoryItem => inventoryItem.Item.itemName == item.itemName);
                if (slot != null) slot.Count++;
                else
                {
                    Debug.LogError("Slot Null");
                }
            }
            else
            {
                var newItem = Instantiate(itemUIPrefab, itemsParent.transform).GetComponent<InventoryItem>();
                newItem.Item = item;
                newItem.Count = 1;
                _items.Add(newItem);
            }
            Usage += item.spaceRequired;
            MissionManager.Instance.UpdateInventoryMissions();
        }

        private void Update()
        {
            tooltip.transform.position = Input.mousePosition;
        }

        public bool CanAddItem(Item item)
        {
            if (item.type != ItemType.Other)
            {
                return Capacity >= item.spaceRequired + Usage && !ContainsItem(item.name);
            }
            return Capacity >= item.spaceRequired + Usage;
        }

        public void RemoveOne(string itemName)
        {
            var item = FindItem(itemName);
            if(item.Count == 1)
            {
                RemoveItem(itemName);
                return;
            }
            item.Count--;
            Usage -= item.Item.spaceRequired;
        }
        
        public void RemoveItem(string itemName)
        {
            //if(!ContainsItem(itemName)) return;
            var item = FindItem(itemName);
            _items.Remove(item);
            Usage -= item.Count * item.Item.spaceRequired;
            Destroy(item.gameObject);
            
        }

        public void ThrowItem(InventoryItem item)
        {
            for (int i = 0; i < item.Count; i++)
            {
                ItemRenderer itemRenderer =
                    Instantiate(itemPrefab, transform.position, Quaternion.identity).GetComponent<ItemRenderer>();
                itemRenderer.item = item.Item;                
            }
        }

        public bool ContainsItem(string itemName)
        {
            return _items.Any(item => item.Item.itemName == itemName);
        }

        public InventoryItem FindItem(string itemName)
        {
            return _items.First(item => item.Item.itemName == itemName);
        }

        public void ShowTooltip(Item item)
        {
            if(item == null) return;
            tooltip.SetActive(true);
            TextMeshProUGUI nameText = tooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = tooltip.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            nameText.SetText(item.itemName);
            descriptionText.SetText(item.description);
        }

        public void HideTooltip()
        {
            tooltip.SetActive(false);
        }

        public void UsePowerUp(Item item)
        {
            if (!ContainsItem(item.itemName) || !CanUsePowerUp) return;
            CanUsePowerUp = false;
            RemoveOne(item.itemName);
            StartCoroutine(PowerUpUse(item));
        }

        private IEnumerator PowerUpUse(Item item)
        {
            powerUpIcon.GetComponent<Image>().sprite = item.icon;
            powerUpIcon.SetActive(true);
            var vitals = GetComponent<PlayerVitals>();
            switch (item.powerUpType)
            {
                case PowerUpType.Hunger:
                    vitals.hunger += (int)item.powerUpMultiplier;
                    break;
                case PowerUpType.Hygiene:
                    vitals.infection += (int) item.powerUpMultiplier;
                    break;
                case PowerUpType.Health:
                    vitals.health += (int) item.powerUpMultiplier;
                    break;
                case PowerUpType.Speed:
                    GetComponent<PlayerMovement>().playerSpeed *= item.powerUpMultiplier;
                    break;
                case PowerUpType.Damage:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return new WaitForSeconds(item.powerUpCooldown);
            if (item.powerUpType == PowerUpType.Speed)
                GetComponent<PlayerMovement>().playerSpeed /= item.powerUpMultiplier;
            powerUpIcon.SetActive(false);
            CanUsePowerUp = true;
        }
    }
}