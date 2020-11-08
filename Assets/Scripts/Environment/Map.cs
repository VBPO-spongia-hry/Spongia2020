using System;
using System.Collections;
using System.Linq;
using Missions;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
    public class Map : MonoBehaviour
    {
        public MapLocation[] locations;
        public MapUI[] mapUis;
        public Transform player;
        public Animation travelAnimation;
        public TextMeshProUGUI travelText;
        public static Map Instance;
        private bool _mapDisabled = false;
        private bool _firstTravel = true;
        public Mission[] powerMissions;
        private AudioSource _audioSource;
        public AudioClip[] musicClips;
        public static bool Disable
        {
            get => Instance._mapDisabled;
            set => Instance._mapDisabled = value;
        }
        [HideInInspector] public MapLocation current;

        private void Start()
        {
            foreach (var location in locations)
            {
                if(location != current) location.gameObject.SetActive(false);
            }

            _audioSource = GetComponent<AudioSource>();
            MapUI.Instances = mapUis.ToList();
        }
        private void Awake()
        {
            current = locations[0];
            if(Instance != null)
            {
                Destroy(Instance.gameObject);
                Debug.LogError("Multiple Map objects detected, destroying old one");
            }
            Instance = this;
        }

        public void Travel(string locationName)
        {
            if(_mapDisabled) return;
            InputHandler.DisableInput = true;
            
            var loc = locations.First(e => e.locationName == locationName);
            if (loc == current) return;
            travelAnimation.Play("TravelShow");
            travelText.SetText($"Traveling to {locationName}");
            StartCoroutine(Travelling(loc));
            if (_firstTravel) StartCoroutine(PowerOutageController());
            _firstTravel = false;
        }

        private IEnumerator Travelling(MapLocation location)
        {
            current = location;
            yield return new WaitWhile(() => travelAnimation.isPlaying);
            foreach (var l in locations)
            {
                l.gameObject.SetActive(false);
            }
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

        private IEnumerator PowerOutageController()
        {
            Debug.Log("outage triggered");
            for (int i = 0; i < powerMissions.Length; i++)
            {
                yield return new WaitForSeconds(Random.Range(5,10));
                TriggerPowerOutage(i);
                yield return new WaitUntil(()=>powerMissions[i].Complete);
                foreach (var location in locations)
                {
                    location.TurnOnPower();
                }
            }
        }
        
        private void TriggerPowerOutage(int i)
        {
            foreach (var location in locations)
            {
                location.TurnOffPower();
            }
            powerMissions[i].Unlock();
            PowerSwitch.Mission = powerMissions[i];
        }

        private void Update()
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = musicClips[Random.Range(0, musicClips.Length)];
                _audioSource.Play();
            }
        }
    }
}