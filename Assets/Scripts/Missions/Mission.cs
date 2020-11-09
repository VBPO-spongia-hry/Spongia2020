using System;
using System.Linq;
using Dialogues;
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
        public bool autoUnlock = true;
        [NonSerialized]
        public bool Complete = false;
        [NonSerialized] public bool Unlocked;
        public Mission[] needToUnlock;
        public string locationName;
        public int completion;
        public Dialogue dialogueOnComplete;
        [NonSerialized] private int _progress = 0;

        public void UpdateProgress()
        {
            if(Complete) return;
            _progress++;
            CheckComplete();
        }

        public void CheckComplete()
        {
            if(!Unlocked) return;
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
            if(!autoUnlock) return;
            Unlocked = needToUnlock.All(e => e.Complete);
            if(Unlocked) MissionManager.Instance.OnMissionUpdate(this);
        }

        public void Unlock()
        {
            Unlocked = true;
            MissionManager.Instance.OnMissionUpdate(this);
        }
        
        public int GetProgress()
        {
            return _progress;
        }

        public void ResetProgress()
        {
            _progress = 0;
            Complete = false;
            Unlocked = false;
        }
    }
}