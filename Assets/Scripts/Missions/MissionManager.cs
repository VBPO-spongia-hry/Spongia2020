using System;
using System.Collections;
using System.Collections.Generic;
using Environment;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Missions
{
    public class MissionManager : MonoBehaviour
    {
        public Mission[] missions;
        [NonSerialized] public List<Mission> active;
        public Animation unlockAnim;
        public TextMeshProUGUI unlockHeader;
        public TextMeshProUGUI unlockContent;
        public GameObject missionPrefab;
        public Transform missionWindow;
        public Slider taskBar;
        private bool _uIshowing;
        private List<GameObject> _unlocked;
        private Queue<Mission> _waitingForUiShow;
        private int _gameProgress;
        public AudioSource audioSource;
        public AudioClip completeClip;

        public static MissionManager Instance;
        private void OnEnable()
        {
            if(Instance != null) Destroy(Instance.gameObject);
            Instance = this;
            _waitingForUiShow = new Queue<Mission>();
        }

        private void Start()
        {
            taskBar.maxValue = missions.Length;
            taskBar.value = 0;
            active = new List<Mission>();
            _unlocked = new List<GameObject>();
            UpdateUnlocked();
        }

        private void Update()
        {
            taskBar.value = Mathf.Lerp(taskBar.value, _gameProgress, Time.deltaTime);            
            if(_waitingForUiShow.Count == 0 || _uIshowing) return;
            Mission mission = _waitingForUiShow.Dequeue();
            Complete(mission);
        }

        public void UpdateInventoryMissions()
        {
            foreach (var mission in missions)
            {
                if (mission.type == MissionType.Inventory && !mission.Complete)
                {
                    mission.CheckComplete();
                }
            }
        }

        private void UpdateUnlocked()
        {
            foreach (var mission in missions)
            {
                if(active.Contains(mission) || mission.Complete) continue;
                mission.CheckUnlocked();
            }
        }

        public void OnMissionUpdate(Mission mission)
        {
            if (mission.Complete)
            {
                active.Remove(mission);
                _gameProgress++;
                audioSource.clip = completeClip;
                audioSource.Play();
                var m = _unlocked.Find(e => e.GetComponent<MissionUI>().mission == mission);
                _unlocked.Remove(m);
                Destroy(m);
                if(_gameProgress == missions.Length) 
                    UIController.Instance.Victory();
                UpdateUnlocked();
            }
            else
            {
                active.Add(mission);
                var missionUi = Instantiate(missionPrefab, missionWindow).GetComponent<MissionUI>();
                missionUi.mission = mission;
                _unlocked.Add(missionUi.gameObject);
            }
            MapUI.UpdateTasks();
            _waitingForUiShow.Enqueue(mission);
        }
        
        private void Complete(Mission mission)
        {
            _uIshowing = true;
            string headerText = mission.Complete ? "Mission Completed" : "Mission Unlocked";
            unlockHeader.SetText(headerText);
            unlockContent.SetText(mission.missionName);
            unlockAnim.Play("UnlockShow");
            StartCoroutine(UnlockHide());
        }

        private IEnumerator UnlockHide()
        {
            yield return new WaitForSeconds(2);
            unlockAnim.Play("UnlockHide");
            yield return new WaitWhile(()=> unlockAnim.isPlaying);
            _uIshowing = false;
        }
    }
}