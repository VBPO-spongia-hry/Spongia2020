using System;
using Missions;
using UnityEngine;

namespace Environment
{
    public class DestroyOnMissionComplete : MonoBehaviour
    {
        public Mission mission;

        private void Update()
        {
            if (mission.Complete)
            {
                Destroy(gameObject);
            }
        }
    }
}