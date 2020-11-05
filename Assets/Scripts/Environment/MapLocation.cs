using System;
using System.Collections;
using System.Security.AccessControl;
using Dialogues;
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
        private bool _hasPower;
        public Light2D ActiveGlobalLight
        {
            get => IsInInterior ? globalInteriorLight : globalExteriorLight;
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
            if (Input.GetKeyDown(KeyCode.T)) StartCoroutine(PowerOutage());
            if (!IsInInterior)
            {
                globalExteriorLight.intensity = dayCycleFunction.Evaluate(_timer * dayCycleSpeed);
            }
            else
            {
                if(_hasPower) globalInteriorLight.intensity = interiorLightIntensity;
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
            Debug.Log("Exited Interior");
        }

        private IEnumerator PowerOutage()
        {
            _hasPower = false;
            for (int i = 0; i < 5; i++)
            {
                globalInteriorLight.intensity = Random.value;
                for (int j = 0; j < Random.Range(1,10); j++)
                {
                    yield return new WaitForEndOfFrame();   
                }
            }
            globalInteriorLight.intensity = .01f;
        }
    }
}