using System;
using System.Collections;
using System.Collections.Generic;
using AOT;
using Missions;
using UnityEngine;

namespace Environment
{
    [Serializable]
    public class SpawnWave
    {
        public int count;
        public GameObject enemyPrefab;
        public float spawnInterval;
        public float endWait;
    }
    public class EnemySpawner : MonoBehaviour
    {
        public SpawnWave[] waves;
        public Transform waypoints;
        private Transform[] _waypointArray;
        public Mission mission;
        public bool complete;
        private int _totalCount;
        public int destroyed = 0;

        public bool Complete => destroyed == _totalCount;
        private void Start()
        {
            _waypointArray = new Transform[waypoints.childCount];
            for (int i = 0; i < waypoints.childCount; i++)
            {
                _waypointArray[i] = waypoints.GetChild(i);
            }

            _totalCount = 0;
            foreach (var wave in waves)
            {
                _totalCount += wave.count;
            }
        }

        private void Update()
        {
            complete = Complete;
            if (Complete)
            {
                if(mission != null)
                {
                    mission.UpdateProgress();
                }
            }
        }

        public void StartSpawning()
        {
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            foreach (var wave in waves)
            {
                for (int i = 0; i < wave.count; i++)
                {
                    Transform transform1;
                    var enemy = Instantiate(wave.enemyPrefab, (transform1 = transform).position, transform1.rotation,
                        transform1).GetComponent<EnemyMovement>();
                    enemy.waypoints = _waypointArray;
                    yield return new WaitForSeconds(wave.spawnInterval);
                }
                yield return new WaitForSeconds(wave.endWait);
            }
        }
    }
}