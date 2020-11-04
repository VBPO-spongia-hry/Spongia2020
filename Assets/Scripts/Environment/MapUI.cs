using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using Missions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Environment
{
    public class MapUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public bool unlocked;
        public GameObject hoverEffect;
        public Map map;
        public MapLocation location;
        public GameObject exclamationMark;
        private Coroutine _tooltipShow;
        private static List<MapUI> _instances;
        
        public static void UpdateTasks()
        {
            foreach (var mapUi in _instances)
            {
                var isNeeded = MissionManager.Instance.active.Any(e =>
                    e.locationName.Split(';').Any(name => name == mapUi.location.locationName));
                mapUi.exclamationMark.SetActive(isNeeded);
            }
        }

        private void OnEnable()
        {
            if(_instances == null) _instances = new List<MapUI>();
            _instances.Add(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(unlocked) hoverEffect.SetActive(true);
            _tooltipShow = StartCoroutine(ShowTooltip());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverEffect.SetActive(false);
            if (_tooltipShow != null) StopCoroutine(_tooltipShow);
            Inventory.Instance.tooltip.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!unlocked || Map.Instance.current == location) return;
            if (_tooltipShow != null) StopCoroutine(_tooltipShow);
            Inventory.Instance.tooltip.gameObject.SetActive(false);
            map.Travel(location.locationName);    
        }

        private IEnumerator ShowTooltip()
        {
            yield return new WaitForSeconds(1);
            var tooltip = Inventory.Instance.tooltip;
            tooltip.SetActive(true);
            TextMeshProUGUI nameText = tooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = tooltip.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            nameText.SetText(location.locationName);
            descriptionText.SetText(location.description);
        }
        
    }
}