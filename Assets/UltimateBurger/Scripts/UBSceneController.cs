using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using vbg;

namespace ub
{
    public class UBSceneController : MonoBehaviour
    {

        public enum State
        {
            MENU,
            MENUWAIT,
            SELECTION,
            WAIT,
            GO,
            RACE,
            END
        }


        public State state = State.WAIT;
        public RectTransform splits;
        public GameObject victoryScreen;
        public GameObject timerUI;
        public GameObject menuUI;
        public GameObject CameraAbove;
        public GameObject CameraP1;
        public GameObject CameraP2;

        public GameObject P1NotReady;
        public GameObject P1Ready;
        public GameObject P2NotReady;
        public GameObject P2Ready;

        public bool enableMoves = false;
        public float launchCountdown = 5.0f;
        public float menuWaitCountdown = 1.0f;
        public Text timerText;

        private bool readyPlayerOne = false;
        private bool readyPlayerTwo = false;

        protected static UBSceneController instance;
        public static UBSceneController Instance
        {
            get
            {
                return instance;
            }
        }

        // Use this for initialization
        void Start()
        {
            instance = this;

        }

        // Update is called once per frame
        void Update()
        {
            switch(state)
            {
                case State.MENU:

                    if (Input.GetButtonDown("Movement_P0") || Input.GetButtonDown("Movement_P2"))
                    {
                        readyPlayerOne = true;
                        P1NotReady.SetActive(false);
                        P1Ready.SetActive(true);
                    }
                    if (Input.GetButtonDown("Movement_P1"))
                    {
                        readyPlayerTwo = true;
                        P2NotReady.SetActive(false);
                        P2Ready.SetActive(true);
                    }

                    if(readyPlayerOne && readyPlayerTwo)
                    {
                        state = State.MENUWAIT;
                    }

                    break;

                case State.MENUWAIT:
                    menuWaitCountdown -= Time.deltaTime;
                    menuWaitCountdown = Mathf.Max(0, menuWaitCountdown);
                    if (menuWaitCountdown == 0)
                    {
                        state = State.WAIT;

                        CameraAbove.SetActive(false);
                        menuUI.SetActive(false);
                        CameraP1.SetActive(true);
                        CameraP2.SetActive(true);
                        timerUI.SetActive(true);
                    }
                    break;
                case State.SELECTION:
                    break;
                case State.WAIT:

                    launchCountdown -= Time.deltaTime;
                    launchCountdown = Mathf.Max(0, launchCountdown);

                    int seconds = Mathf.FloorToInt(launchCountdown);
                    int ms = Mathf.FloorToInt((launchCountdown - seconds) * 100);

                    timerText.text = string.Format("{0:00}:{1:00}", seconds, ms);

                    if (launchCountdown == 0)
                    {
                        timerUI.SetActive(false);
                        state = State.GO;
                        enableMoves = true;
                    }
                    break;
            }
        }

        public void OnDeath(VBGCharacterController player)
        {
            if(state == State.END)
                return;
            state = State.END;
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