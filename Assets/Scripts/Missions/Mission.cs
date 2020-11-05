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
        public string description;
        public string itemName;
        [NonSerialized]
        public bool Complete = false;
        [NonSerialized] private bool _unlocked;
        public Mission[] needToUnlock;
        public int failTime;
        public string locationName;
        public int completion;
        [NonSerialized] private int _progress = 0;

        public void UpdateProgress()
        {
            if(Complete) return;
            _progress++;
            CheckComplete();
        }

        public void CheckComplete()
        {
            switch (type)
            {
                case MissionType.Inventory:
                    Complete = Inventory.Instance.ContainsItem(itemName);
                    break;
                case MissionType.Destination:
                    Complete = _progress >= completion;
                    break;
                case MissionType.Combat:
                    Complete = _progress >= completion;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Complete)
            {
                MissionManager.Instance.OnMissionUpdate(this);
            }
        }

        public void CheckUnlocked()
        {
            _unlocked = needToUnlock.All(e => e.Complete);
            if(_unlocked) MissionManager.Instance.OnMissionUpdate(this);
        }

        public int GetProgress()
        {
            return _progress;
        }

        public void ResetProgress()
        {
            _progress = 0;
            Complete = false;
            _unlocked = false;
        }
    }
}