using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class GameManager : MonoBehaviour
    {

        public bool showCollidersInGame = DebugConstants.SHOW_COLLIDERS_INGAME;
        public struct DebugConstants
        {
            public readonly static bool SHOW_COLLIDERS_INGAME = true;
        }

        public struct Constants
        {
            public readonly static string TAG_CHARACTER = "Player";
            public readonly static string TAG_DYNAMIC = "Dynamic";
            public readonly static string TAG_ENNEMY = "Ennemy";
            public readonly static string TAG_GAMEEFFECT = "GameEffect";

            public readonly static string LAYER_COLLIDERS = "Colliders";

            public readonly static float HUD_COMPASS_FADEOUT = 0.2f;    // s
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
            return startPoints[_idx % startPoints.Count];
        }
    }
}