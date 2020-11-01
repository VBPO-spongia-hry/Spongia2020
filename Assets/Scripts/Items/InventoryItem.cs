using System;
using System.Collections;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Items
{
    public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private Item _item;
        public Item Item
        {
            get => _item;
            set
            {
                _item = value;
                GetComponentInChildren<Image>().sprite = _item == null ? null : _item.icon;
            }
        }

        public bool isLoadoutSlot;
        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                countText.text = _count.ToString();
            }
        }
        public Text countText;
        private static Coroutine _tooltipRoutine;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _tooltipRoutine = StartCoroutine(ShowTooltip());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_tooltipRoutine != null) StopCoroutine(_tooltipRoutine);
            Inventory.Instance.HideTooltip();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isLoadoutSlot)
            {
                switch (eventData.button)
                {
                    case PointerEventData.InputButton.Left:
                        Equip();
                        break;
                    case PointerEventData.InputButton.Right:
                        Inventory.Instance.ThrowItem(Inventory.Instance.FindItem(_item.itemName));
                        Inventory.Instance.RemoveItem(_item.itemName);
                        break;
                    case PointerEventData.InputButton.Middle:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (_item != null)
            {
                if (Inventory.Instance.CanAddItem(Item))
                {
                    Inventory.Instance.AddItem(Item);
                    Item = null;
                }
            }
            
        }

        private void Equip()
        {
            switch (Item.type)
            {
                case ItemType.Protection:
                    if(Inventory.Instance.Protection != null) return;
                    Inventory.Instance.Protection = _item;
                    break;
                case ItemType.Weapon:
                    if(Inventory.Instance.Weapon != null) return;
                    Inventory.Instance.Weapon = _item;
                    break;
                case ItemType.Armor:
                    if(Inventory.Instance.Armor != null) return;
                    Inventory.Instance.Armor = _item;
                    break;
                case ItemType.InventoryUpgrade:
                    if(Inventory.Instance.Backpack != null) return;
                    Inventory.Instance.Backpack = _item;
                    break;
                case ItemType.Other:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Inventory.Instance.RemoveItem(Item.itemName);
        }

        private IEnumerator ShowTooltip()
        {
            yield return new WaitForSeconds(1);
            Inventory.Instance.ShowTooltip(Item);
        }
    }
}