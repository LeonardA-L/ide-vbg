using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace vbg
{
    public class ArenaController : MonoBehaviour
    {
        public CharacterHealth characterHealth;
        public float timer;
        private float timerMax;
        public string timerActiveSwitch;

        public Text timerText;
        public GameObject arenaHud;

        private List<VBGCharacterController> players = null;

        // Use this for initialization
        void Start()
        {
            timerMax = timer;
        }

        // Update is called once per frame
        void Update()
        {
            bool arenaActive = SwitchManager.Instance.GetSwitch(timerActiveSwitch);
            arenaHud.SetActive(arenaActive);

            if (arenaActive)
            {

                timer -= Time.deltaTime;
                timer = Mathf.Max(timer, 0.0f);
                SwitchManager.Instance.SetValue("ArenaTimer", timer);
                SwitchManager.Instance.SetValue("ArenaTimerRatio", timer / timerMax);

                SwitchManager.Instance.SetValue("ArenaCoreLife", characterHealth.health);
                SwitchManager.Instance.SetValue("ArenaCoreLifeRatio", characterHealth.health / characterHealth.MaxHealth);

                int minutes = Mathf.FloorToInt(timer / 60F);
                int seconds = Mathf.FloorToInt(timer - minutes * 60);

                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            if(players == null)
            {
                players = PlayerManager.Instance.GetAllPlayersInGame();
            }
            bool allDeads = players.Count > 0;
            foreach(VBGCharacterController p in players)
            {
                allDeads &= p.IsDead();
            }
            if(allDeads)
            {
                GameManager.Instance.Death(arenaActive);
            }
        }

        public void ResetArena()
        {
            
        }
    }
}