﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class PlayerManager : MonoBehaviour
    {
        private static readonly string PLAYERS_INGAME_VALUE = "PlayersInGame";
        private bool valueInit = false;

        public struct Constants
        {
            // Max number of players allowed in game at a given time
            public readonly static int PLAYERS_MAX = 4;
            // Highest gamepad ID we're listening input from
            public readonly static int CONTROLLER_LISTENER_MAX = 8;    // 0: keyboard, 1-8 (included): gamepads
        }

        protected static PlayerManager instance;
        public static PlayerManager Instance
        {
            get
            {
                return instance;
            }
        }

        // Players currently in game
        private int playersInGame = 0;
        private List<VBGCharacterController> players;

        private Dictionary<int, CharacterUserControl> controllersInGame;

        // ----- Public

        public GameObject aresPrefab;
        public GameObject artemisPrefab;
        public GameObject hephaestusPrefab;
        public GameObject apolloPrefab;

        void Start()
        {
            instance = this;
            controllersInGame = new Dictionary<int, CharacterUserControl>();
            players = new List<VBGCharacterController>();
        }

        void Update()
        {
            if(!valueInit && SwitchManager.Instance != null)
            {
                SwitchManager.Instance.SetValue(PLAYERS_INGAME_VALUE, 0);
                valueInit = true;
            }
            if (playersInGame < Constants.PLAYERS_MAX)   // Check new players joining the game
            {
                for (int controllerID = 0; controllerID <= Constants.CONTROLLER_LISTENER_MAX; controllerID++)
                {
                    if (controllersInGame.ContainsKey(controllerID))
                        continue;

                    if (Input.GetButtonDown("Activate_P" + controllerID))
                    {
                        InitPlayer(controllerID);
                    }
                }
            }

            // update player values
            for (int i = 0; i < players.Count; i++)
            {
                VBGCharacterController player = players[i];

                try
                {
                    float health = player.GetHealth().GetHealth();
                    SwitchManager.Instance.SetValue("P" + i + "_health", health);
                    SwitchManager.Instance.SetValue("P" + i + "_healthMax", player.GetHealth().MaxHealth);
                    SwitchManager.Instance.SetValue("P" + i + "_healthRatio", health / player.GetHealth().MaxHealth);
                    SwitchManager.Instance.SetSwitch("P" + i, true);

                    float sAttackTimer = player.specialAttack.timer;
                    float sAttackCooldown = player.specialAttack.cooldown;
                    SwitchManager.Instance.SetValue("P" + i + "_sAttackTimer", sAttackCooldown - sAttackTimer);
                    SwitchManager.Instance.SetValue("P" + i + "_sAttackCooldown", sAttackCooldown);
                    SwitchManager.Instance.SetValue("P" + i + "_sAttackRatio", (sAttackCooldown - sAttackTimer) / sAttackCooldown);
                    SwitchManager.Instance.SetSwitch("P" + i + "_sAttackUnavailable", sAttackTimer > 0.0f);

                    float sDefenseTimer = player.specialDefense.timer;
                    float sDefenseCooldown = player.specialDefense.cooldown;
                    SwitchManager.Instance.SetValue("P" + i + "_sDefenseTimer", sDefenseCooldown - sDefenseTimer);
                    SwitchManager.Instance.SetValue("P" + i + "_sDefenseCooldown", sDefenseCooldown);
                    SwitchManager.Instance.SetValue("P" + i + "_sDefenseRatio", (sDefenseCooldown - sDefenseTimer) / sDefenseCooldown);
                    SwitchManager.Instance.SetSwitch("P" + i + "_sDefenseUnavailable", sDefenseTimer > 0.0f);

                    float sMovementTimer = player.specialMovement.timer;
                    float sMovementCooldown = player.specialMovement.cooldown;
                    SwitchManager.Instance.SetValue("P" + i + "_sMovementTimer", sMovementCooldown - sMovementTimer);
                    SwitchManager.Instance.SetValue("P" + i + "_sMovementCooldown", sMovementCooldown);
                    SwitchManager.Instance.SetValue("P" + i + "_sMovementRatio", (sMovementCooldown - sMovementTimer) / sMovementCooldown);
                    SwitchManager.Instance.SetSwitch("P" + i + "_sMovementUnavailable", sMovementTimer > 0.0f);


                }
                catch (Exception e)
                {

                }
            }
        }

        private void InitPlayer(int _controllerID)
        {
            int playerID = playersInGame;
            Debug.Log("Activating Controller " + _controllerID + " Player " + playerID);
            GameObject characterPrefab;
            switch (playerID)
            {
                case 1:
                    characterPrefab = artemisPrefab;
                    break;
                case 0:
                    characterPrefab = hephaestusPrefab;
                    break;
                case 2:
                    characterPrefab = apolloPrefab;
                    break;
                case 3:
                default:
                    characterPrefab = aresPrefab;
                    break;
            }

            GameObject player = GameObject.Instantiate(characterPrefab);
            //player.transform.position = new Vector3(playerID * 1.0f, 0.0f, 0.0f);
            SpawnPoint startPoint = GameManager.Instance.GetStartPoint(playerID);
            player.transform.position = startPoint.transform.position;
            player.transform.rotation = startPoint.transform.rotation;
            player.name = "Player " + playerID;

            CharacterUserControl tpuc = player.GetComponent<CharacterUserControl>();
            CharacterVibrationController tpv = player.GetComponent<CharacterVibrationController>();
            tpuc.SetController(_controllerID);
            tpv.SetController(_controllerID);

            controllersInGame.Add(_controllerID, tpuc);
            players.Add(tpuc.GetComponent<VBGCharacterController>());

            playersInGame++;

            SwitchManager.Instance.SetValue(PLAYERS_INGAME_VALUE, playersInGame);

            VSFightingMap vs = GetComponent<VSFightingMap>();
            if (vs != null)
            {
                vs.NewPlayer(tpuc.GetComponent<VBGCharacterController>());
            }
        }

        public Vector3 GetPlayerBarycenter()
        {
            Vector3 barycenter = new Vector3();

            foreach(VBGCharacterController player in players) {
                barycenter += player.transform.position;
            }
            barycenter /= playersInGame;

            return barycenter;
        }

        public float GetPlayersRadius()
        {
            Vector3 barycenter = GetPlayerBarycenter();
            float max = 0;

            foreach (VBGCharacterController player in players)
            {
                if((player.transform.position - barycenter).magnitude > max)
                {
                    max = (player.transform.position - barycenter).magnitude;
                }
            }
            return max;
        }

        public int GetPlayersInGameAmount()
        {
            return playersInGame;
        }

        public List<VBGCharacterController> GetAllPlayersInGame()
        {
            return players;
        }
    }
}