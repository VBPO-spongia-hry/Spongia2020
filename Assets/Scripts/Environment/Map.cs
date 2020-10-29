using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Environment
{
    public class Map : MonoBehaviour
    {
        public MapLocation[] locations;
        public Transform player;
        public Animation travelAnimation;
        public TextMeshProUGUI travelText;
        public static Map Instance;
        [HideInInspector] public MapLocation current;

        private void Start()
        {
            current = locations[0];
            foreach (var location in locations)
            {
                if(location != current) location.gameObject.SetActive(false);
            }
        }
        private void Awake()
        {
            if(Instance != null) Destroy(Instance.gameObject);
            Instance = this;
        }

        public void Travel(string locationName)
        {
            InputHandler.DisableInput = true;
            foreach (var location in locations)
            {
                location.gameObject.SetActive(false);
            }
            var loc = locations.First(e => e.locationName == locationName);
            if (loc == current) return;
            travelAnimation.Play("TravelShow");
            travelText.SetText($"Traveling to {locationName}");
            StartCoroutine(Travelling(loc));
        }

        private IEnumerator Travelling(MapLocation location)
        {
            current = location;
            yield return new WaitWhile(() => travelAnimation.isPlaying);
            location.gameObject.SetActive(true);
            var position = location.spawnLocation.position;
            player.position = position;
            if (Camera.main != null) Camera.main.transform.position = position;
            UIController.Instance.HideUI();
            yield return new WaitForSeconds(location.travelTime);
            travelAnimation.Play("TravelHide");
            yield return new WaitWhile(() => travelAnimation.isPlaying);
            
            InputHandler.DisableInput = false;
        }
    }
}