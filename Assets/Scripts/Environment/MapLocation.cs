using System;
using System.Collections;
using System.Security.AccessControl;
using Dialogues;
using Items;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Random = UnityEngine.Random;

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
        [NonSerialized] public bool IsInInterior;
        private float _timer;
        public bool hasPower;
        public Light2D ActiveGlobalLight
        {
            get => IsInInterior ? globalInteriorLight : globalExteriorLight;
        }

        public void TurnOnPower()
        {
            hasPower = true;
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

        public void AddItemToInventory(Item item)
        {
            Inventory.Instance.AddItem(item);
        }

        public void BeginDialogue(Dialogue dialogue)
        {
            DialogueManager.Singleton.BeginDialogue(dialogue);
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (!IsInInterior)
            {
                globalExteriorLight.intensity = dayCycleFunction.Evaluate(_timer * dayCycleSpeed);
            }
            else
            {
                if(hasPower) globalInteriorLight.intensity = interiorLightIntensity;
            }
        }

        public void InteriorEnter()
        {
            globalExteriorLight.gameObject.SetActive(false);
            globalInteriorLight.gameObject.SetActive(true);
            IsInInterior = true;
            Debug.Log(globalInteriorLight.intensity);
        }

        public void ExteriorEnter()
        {
            globalExteriorLight.gameObject.SetActive(true);
            globalInteriorLight.gameObject.SetActive(false);
            IsInInterior = false;
        }

        private IEnumerator PowerOutage()
        {
            
            for (int i = 0; i < 10; i++)
            {
                globalInteriorLight.intensity = Random.value;
                for (int j = 0; j < Random.Range(1,20); j++)
                {
                    yield return new WaitForEndOfFrame();   
                }
            }
            globalInteriorLight.intensity = .01f;
        }

        public void TurnOffPower()
        {
            hasPower = false;
            if(isActiveAndEnabled) StartCoroutine(PowerOutage());
            else globalInteriorLight.intensity = .01f;
        }
    }
}