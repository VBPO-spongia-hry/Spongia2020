using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Missions
{
    public class MissionManager : MonoBehaviour
    {
        public Mission[] missions;
        public List<Mission> active;
        public Animation unlockAnim;
        public TextMeshProUGUI unlockHeader;
        public TextMeshProUGUI unlockContent;
        public Slider taskBar;

        private bool _uIshowing;
        private List<Mission> _unlocked;
        private Queue<Mission> _waitingForUiShow;
        private int _gameProgress;

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
                if (mission.type == MissionType.Inventory && !mission.complete)
                {
                    mission.CheckComplete();
                }
            }
        }

        private void UpdateUnlocked()
        {
            foreach (var mission in missions)
            {
                if(active.Contains(mission) || mission.complete) continue;
                mission.CheckUnlocked();
            }
        }

        public void OnMissionUpdate(Mission mission)
        {
            if (mission.complete)
            {
                active.Remove(mission);
                _gameProgress++;
                UpdateUnlocked();
            }
            else active.Add(mission);
            _waitingForUiShow.Enqueue(mission);
        }
        
        private void Complete(Mission mission)
        {
            _uIshowing = true;
            string headerText = mission.complete ? "Mission Completed" : "Mission Unlocked";
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