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

        public struct Constants
        {
            public readonly static string TAG_CHARACTER = "Player";
            public readonly static string TAG_NONPLAYER_CHARACTER = "NonPlayerCharacter";
            public readonly static string TAG_GAMEEFFECT = "GameEffect";

        }

        protected static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                return instance;
            }
        }

        private Dictionary<string, bool> switches;
        public delegate void SwitchCallback(bool switchValue);
        private Dictionary<string, List<SwitchCallback>> switchesPubSub;

        private List<SpawnPoint> spawnPoints;
        private List<SpawnPoint> startPoints;

        // Use this for initialization
        void Start()
        {
            instance = this;
            spawnPoints = new List<SpawnPoint>();
            startPoints = new List<SpawnPoint>();
            switches = new Dictionary<string, bool>();
            switchesPubSub = new Dictionary<string, List<SwitchCallback>>();

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

        public void RegisterSwitchListener(string name, SwitchCallback callback)
        {
            if (!switchesPubSub.ContainsKey(name))
            {
                switchesPubSub.Add(name, new List<SwitchCallback>());
            }
            switchesPubSub[name].Add(callback);
        }

        public bool GetSwitch(string name)
        {
            bool ret = false;
            switches.TryGetValue(name, out ret);
            return ret;
        }

        public void SetSwitch(string name, bool value)
        {
            switches[name] = value;
            List<SwitchCallback> subscribers = null;

            switchesPubSub.TryGetValue(name, out subscribers);

            if(subscribers != null)
            {
                foreach (SwitchCallback sw in subscribers) {
                    sw(value);
                }
            }
        }
    }
}