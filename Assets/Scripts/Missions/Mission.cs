using System;
using System.Linq;
using Environment;
using Items;
using UnityEngine;

namespace Missions
{
    [Serializable]
    public enum MissionType
    {
        Inventory,
        Destination,
        Combat,
    }
    [CreateAssetMenu(fileName = "Mission", menuName = "Mission", order = 3)]
    public class Mission : ScriptableObject
    {
        public MissionType type;
        public string missionName;
        public string itemName;
        public bool complete;
        public bool unlocked;
        public Mission[] needToUnlock;
        public int failTime;
        public string locationName;
        public int completion;
        private int _progress;

        public void UpdateProgress()
        {
            if(complete) return;
            _progress++;
            CheckComplete();
        }

        public void CheckComplete()
        {
            switch (type)
            {
                case MissionType.Inventory:
                    complete = Inventory.Instance.ContainsItem(itemName);
                    break;
                case MissionType.Destination:
                    complete = _progress >= completion;
                    break;
                case MissionType.Combat:
                    complete = _progress >= completion;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (complete)
            {
                MissionManager.Instance.OnMissionUpdate(this);
            }
        }

        public void CheckUnlocked()
        {
            unlocked = needToUnlock.All(e => e.unlocked);
            if(unlocked) MissionManager.Instance.OnMissionUpdate(this);
        }
    }
}