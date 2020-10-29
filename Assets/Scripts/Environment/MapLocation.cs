using UnityEngine;

namespace Environment
{
    public class MapLocation : MonoBehaviour
    {
        public string locationName;
        public string description;
        public float travelTime;
        public Transform spawnLocation;
        public Pathfinding pathfinding;

    }
}