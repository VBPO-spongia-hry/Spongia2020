using System;
using TMPro;
using UnityEngine;

namespace Missions
{
    public class MissionUI : MonoBehaviour
    {
        public Mission mission;
        public TextMeshProUGUI headerText;
        public TextMeshProUGUI descriptionText;

        private void Start()
        {
            headerText.SetText($"{mission.missionName}: {mission.locationName.Replace(";", ", ")}");
            descriptionText.SetText($"{mission.description} {GetMissionProgress()}");
        }

        private void Update()
        {
            descriptionText.SetText($"{mission.description} {GetMissionProgress()}");
            if(mission.Complete) Destroy(gameObject);
        }

        private string GetMissionProgress()
        {
            if (mission.completion == 1) return "";
            return $"({mission.GetProgress()}/{mission.completion})";
        }
    }
}