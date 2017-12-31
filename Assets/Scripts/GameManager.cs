using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour {

    private int playersInGame = 0;
    public readonly static int PLAYERS_MAX = 4;

    private Dictionary<int, ThirdPersonUserControl> players;

    public GameObject characterPrefab;

    // Use this for initialization
    void Start () {
        players = new Dictionary<int, ThirdPersonUserControl>();

    }
	
	// Update is called once per frame
	void Update () {
        if(playersInGame < PLAYERS_MAX)
        {
            for (int i = 0; i <= 8; i++)
            {
                if(players.ContainsKey(i))
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

    private void InitPlayer(int _playerId)
    {
        Debug.Log("Activating Player " + _playerId);
        GameObject player = (GameObject)GameObject.Instantiate(characterPrefab);
        player.transform.position = new Vector3(_playerId * 1.0f, 0.0f, 0.0f);
        player.name = "Player " + _playerId;

        ThirdPersonUserControl tpuc = player.GetComponent<ThirdPersonUserControl>();
        tpuc.SetController(_playerId);

        players.Add(_playerId, tpuc);

        playersInGame++;
    }
}
