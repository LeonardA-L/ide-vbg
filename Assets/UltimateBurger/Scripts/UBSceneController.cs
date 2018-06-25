using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vbg;

namespace ub
{
    public class UBSceneController : MonoBehaviour
    {

        public enum State
        {
            MENU,
            SELECTION,
            WAIT,
            GO,
            RACE,
            END
        }

        public State state = State.GO;
        public RectTransform splits;
        public GameObject victoryScreen;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnDeath(VBGCharacterController player)
        {
            if(state == State.END)
                return;
            victoryScreen.SetActive(true);

            Time.timeScale = 0.2f;

            UBCharacterController cc = (UBCharacterController)player;
            if(cc.playerID == 1)
            {
                splits.localPosition = new Vector2(-960, 0);
            }
        }
    }
}