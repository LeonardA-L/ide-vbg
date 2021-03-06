﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

namespace vbg
{
    public class GameManager : MonoBehaviour
    {

        public bool showCollidersInGame = DebugConstants.SHOW_COLLIDERS_INGAME;
        public bool allowRevive = true;
        public bool isArena = false;
        public bool isQuitting = false;
        private bool pause = false;
        private bool pauseButtonIsDown = false;
        private bool endScreen = false;

        public GameObject victoryScreen;
        public GameObject deathScreen;
        public GameObject deathArenaScreen;

        public struct DebugConstants
        {
            public readonly static bool SHOW_COLLIDERS_INGAME = true;
        }

        public struct Constants
        {
            public readonly static string TAG_PLAYER = "Player";
            public readonly static string TAG_DYNAMIC = "Dynamic";
            public readonly static string TAG_ENNEMY = "Ennemy";
            public readonly static string TAG_GAMEEFFECT = "GameEffect";
            public readonly static string TAG_NONTRIGGERCOLLIDER = "NonTriggerCollider";

            public readonly static string LAYER_COLLIDERS = "Colliders";
            public readonly static int LAYER_ARROW = 15;
            public readonly static int LAYER_SOUND = 14;

            public readonly static float HUD_COMPASS_FADEOUT = 0.2f;    // s

            public readonly static float FPS_REF = 50.0f;

            public readonly static float CHAOS_MASS_REFERENCE = 40;
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

        private UIController uiController;

        // Use this for initialization
        void Start()
        {
            instance = this;
            spawnPoints = new List<SpawnPoint>();
            startPoints = new List<SpawnPoint>();

            uiController = GetComponent<UIController>();

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

            if (!endScreen && !pauseButtonIsDown && Input.GetButton("Pause"))
            {
                pauseButtonIsDown = true;
                if (pause)
                {
                    Play();
                }
                else
                {
                    Pause();
                }
            }
            pauseButtonIsDown = Input.GetButton("Pause");

        }

        public SpawnPoint GetStartPoint(int _idx)
        {
            return startPoints[_idx % startPoints.Count];
        }

        public void Pause(bool _showEndScreen = true)
        {
            Time.timeScale = 0;
            uiController.SetPauseMenu(_showEndScreen);
            pause = true;
        }

        public void Play()
        {
            Time.timeScale = 1;
            uiController.SetPauseMenu(false);
            pause = false;
        }

        public void OnDeath(VBGCharacterController player)
        {
            VSFightingMap vs = GetComponent<VSFightingMap>();
            if(vs != null)
            {
                vs.OnDeath(player);
            }

            ub.UBSceneController ub = GetComponent<ub.UBSceneController>();
            if (ub != null)
            {
                ub.OnDeath(player);
            }
        }

        private void OnApplicationQuit()
        {
            for(int i=0;i<8;i++)
            {
                GamePad.SetVibration((PlayerIndex)i, 0, 0);
            }
            isQuitting = true;
        }

        public void AddChaos(float _diff)
        {
            float currentChaos = SwitchManager.Instance.GetValue("Chaos") ?? 0;
            SwitchManager.Instance.SetValue("Chaos", currentChaos + _diff);
        }

        public Transform GetNearestSpawnPoint(Transform t)
        {
            Transform ret = spawnPoints[0].transform;

            float distance = float.MaxValue;
            foreach(SpawnPoint s in spawnPoints)
            {
                float d = (s.transform.position - t.position).magnitude;
                if (d < distance)
                {
                    distance = d;
                    ret = s.transform;
                }
            }

            return ret;
        }

        public void Death(bool _arena)
        {
            if (endScreen)
                return;
            Pause(false);
            endScreen = true;
            if (_arena)
            {
                deathArenaScreen.SetActive(true);
            }
            else
            {
                deathScreen.SetActive(true);
            }
            SoundManager.Instance.PostEvent("Play_GAME_OVER", gameObject);
        }

        public void Undeath()
        {
            endScreen = false;
            deathArenaScreen.SetActive(false);
            deathScreen.SetActive(false);
            Play();

        }

        public void Victory()
        {
            Pause(false);
            endScreen = true;
            victoryScreen.SetActive(true);
            SoundManager.Instance.PostEvent("Play_VICTOIRE", gameObject);
        }
    }
}