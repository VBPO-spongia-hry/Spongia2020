using System;
using System.Security.AccessControl;
using Dialogues;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Environment
{
    public class MapLocation : MonoBehaviour
    {
        public string locationName;
        public string description;
        public float travelTime;
        public Transform spawnLocation;
        public Pathfinding pathfinding;
        public Light2D globalInteriorLight;
        public Light2D globalExteriorLight;
        public float interiorLightIntensity;
        public float dayCycleSpeed;
        public AnimationCurve dayCycleFunction;
        private bool _isInInterior;
        private float _timer;
        [NonSerialized] public bool HasPower;
        public Light2D ActiveGlobalLight
        {
            get => _isInInterior ? globalInteriorLight : globalExteriorLight;
        }

        private void Start()
        {
            globalExteriorLight.gameObject.SetActive(true);
            globalInteriorLight.gameObject.SetActive(false);
        }

        public void DisableMap()
        {
            Map.Disable = true;
        }

        public void EnableMap()
        {
            Map.Disable = false;
        }

        public void BeginDialogue(Dialogue dialogue)
        {
            DialogueManager.Singleton.BeginDialogue(dialogue);
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (!_isInInterior)
            {
                globalExteriorLight.intensity = dayCycleFunction.Evaluate(_timer * dayCycleSpeed);
            }
            else
            {
                globalInteriorLight.intensity = interiorLightIntensity;
            }
        }

        public void InteriorEnter()
        {
            globalExteriorLight.gameObject.SetActive(false);
            globalInteriorLight.gameObject.SetActive(true);
            _isInInterior = true;
            Debug.Log(globalInteriorLight.intensity);
        }

        public void ExteriorEnter()
        {
            globalExteriorLight.gameObject.SetActive(true);
            globalInteriorLight.gameObject.SetActive(false);
            _isInInterior = false;
            Debug.Log("Exited Interior");
        }
    }
}