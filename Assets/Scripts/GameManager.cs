using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour {

    private int playersInGame = 0;
    public readonly static int PLAYERS_MAX = 4;

    private Dictionary<int, ThirdPersonUserControl> controllersInGame;
    private List<ThirdPersonUserControl> players;

    public GameObject characterPrefab;

    // Use this for initialization
    void Start () {
        controllersInGame = new Dictionary<int, ThirdPersonUserControl>();
        players = new List<ThirdPersonUserControl>();

    }
	
	// Update is called once per frame
	void Update () {
        if(playersInGame < PLAYERS_MAX)
        {
            for (int i = 0; i <= 8; i++)
            {
                if(controllersInGame.ContainsKey(i))
                {
                    continue;
                }
                if (Input.GetButtonDown("Activate_P"+i))
                {
                    InitPlayer(i);
                }
            }
        }
    }

    private void InitPlayer(int _controllerID)
    {
        int playerID = playersInGame;
        Debug.Log("Activating Controller " + _controllerID + " Player " + playerID);
        GameObject player = (GameObject)GameObject.Instantiate(characterPrefab);
        player.transform.position = new Vector3(playerID * 1.0f, 0.0f, 0.0f);
        player.name = "Player " + playerID;

        ThirdPersonUserControl tpuc = player.GetComponent<ThirdPersonUserControl>();
        tpuc.SetController(_controllerID);

        controllersInGame.Add(_controllerID, tpuc);
        players.Add(tpuc);

        playersInGame++;
    }
}
