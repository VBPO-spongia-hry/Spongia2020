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
                GetComponentInChildren<Image>().sprite = _item.icon;
            }
        }

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
            switch (Item.type)
            {
                case ItemType.Protection:
                    Inventory.Instance.Protection = _item;
                    break;
                case ItemType.Weapon:
                    Inventory.Instance.weapon = _item;
                    break;
                case ItemType.Armor:
                    Inventory.Instance.Armor = _item;
                    break;
                case ItemType.InventoryUpgrade:
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