using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

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

        public void DisableMap()
        {
            Map.Disable = true;
        }

        public void EnableMap()
        {
            Map.Disable = false;
        }

        public void InteriorEnter()
        {
            
        }

        public void ExteriorEnter()
        {
            
        }
    }
}