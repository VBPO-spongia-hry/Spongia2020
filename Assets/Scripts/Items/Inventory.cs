using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.InteropServices.WindowsRuntime;
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

        private void Start()
        {
            Usage = 0;
            capacitySlider.maxValue = 100;
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
        public Item weapon;
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
        
        public void RemoveItem(string itemName)
        {
            if(!ContainsItem(itemName)) return;
            foreach (var item in _items)
            {
                if (item.Item.name == itemName)
                {
                    _items.Remove(item);
                    Destroy(item.gameObject);
                    ItemRenderer itemRenderer = Instantiate(itemPrefab, transform.position, Quaternion.identity).GetComponent<ItemRenderer>();
                    itemRenderer.item = item.Item;
                    break;
                }
            }
        }

        private bool ContainsItem(string itemName)
        {
            return _items.Any(item => item.Item.name == itemName);
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
    }
}