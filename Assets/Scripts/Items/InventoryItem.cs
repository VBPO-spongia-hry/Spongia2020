using System;
using System.Collections;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Items
{
    public delegate void ItemMouseHandler();
    public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private Item _item;
        public Button useButton;
        public event ItemMouseHandler OnRightClick;
        public event ItemMouseHandler OnLeftClick;
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

        private void Start()
        {
            if (!isLoadoutSlot && useButton != null) useButton.gameObject.SetActive(Item.type == ItemType.PowerUp);
        }

        private void Update()
        {
            if(!isLoadoutSlot && useButton != null) useButton.interactable = Inventory.Instance.CanUsePowerUp;
        }

        public void UsePowerUp()
        {
            if(_tooltipRoutine != null) StopCoroutine(_tooltipRoutine);
            Inventory.Instance.HideTooltip();
            Inventory.Instance.UsePowerUp(Item);
        }

        public Text countText;
        private static Coroutine _tooltipRoutine;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _tooltipRoutine = StartCoroutine(ShowTooltip());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideTooltip();
        }

        public void HideTooltip()
        {
            if (_tooltipRoutine != null) StopCoroutine(_tooltipRoutine);
            Inventory.Instance.HideTooltip();
        }

        public void AddInventoryEventListeners()
        {
            OnLeftClick += Equip;
            OnRightClick += RemoveItem;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isLoadoutSlot)
            {
                switch (eventData.button)
                {
                    case PointerEventData.InputButton.Left:
                        OnLeftClick?.Invoke();
                        break;
                    case PointerEventData.InputButton.Right:
                        OnRightClick?.Invoke();
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

        private void RemoveItem()
        {
            Inventory.Instance.ThrowItem(Inventory.Instance.FindItem(_item.itemName));
            Inventory.Instance.RemoveOne(_item.itemName);
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
                case ItemType.PowerUp:
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

        private void OnDestroy()
        {
            if(_tooltipRoutine != null) StopCoroutine(_tooltipRoutine);
            Inventory.Instance.HideTooltip();
        }
    }
}