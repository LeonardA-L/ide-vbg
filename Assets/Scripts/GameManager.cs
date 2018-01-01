﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class GameManager : MonoBehaviour
    {
        public struct DebugConstants
        {
            public readonly static bool SHOW_COLLIDERS_INGAME = true;
        }

        protected static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                return instance;
            }
        }

        private List<SpawnPoint> spawnPoints;
        private List<SpawnPoint> startPoints;

        // Use this for initialization
        void Start()
        {
            instance = this;
            spawnPoints = new List<SpawnPoint>();
            startPoints = new List<SpawnPoint>();

            SpawnPoint[] spawns = (SpawnPoint[])FindObjectsOfType(typeof(SpawnPoint));
            foreach (SpawnPoint spawn in spawns)
            {
                spawnPoints.Add(spawn);
                if (spawn.IsStartPoint())
                {
                    startPoints.Add(spawn);
                }
            }

            Debug.Log("Game initialized. " + spawnPoints.Count + " spawn points including " + startPoints.Count + " start points.");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public SpawnPoint GetStartPoint(int _idx)
        {
            Debug.Log("Return " + startPoints[_idx % startPoints.Count]);
            return startPoints[_idx % startPoints.Count];
        }
    }
}