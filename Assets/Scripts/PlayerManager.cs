using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class PlayerManager : MonoBehaviour
    {

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
        private List<CharacterUserControl> players;

        private Dictionary<int, CharacterUserControl> controllersInGame;

        // ----- Public

        public GameObject characterPrefab;

        void Start()
        {
            instance = this;
            controllersInGame = new Dictionary<int, CharacterUserControl>();
            players = new List<CharacterUserControl>();

        }

        void Update()
        {
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
        }

        private void InitPlayer(int _controllerID)
        {
            int playerID = playersInGame;
            Debug.Log("Activating Controller " + _controllerID + " Player " + playerID);
            GameObject player = GameObject.Instantiate(characterPrefab);
            //player.transform.position = new Vector3(playerID * 1.0f, 0.0f, 0.0f);
            SpawnPoint startPoint = GameManager.Instance.GetStartPoint(playerID);
            player.transform.position = startPoint.transform.position;
            player.transform.rotation = startPoint.transform.rotation;
            player.name = "Player " + playerID;

            CharacterUserControl tpuc = player.GetComponent<CharacterUserControl>();
            tpuc.SetController(_controllerID);

            controllersInGame.Add(_controllerID, tpuc);
            players.Add(tpuc);

            playersInGame++;
        }
    }
}