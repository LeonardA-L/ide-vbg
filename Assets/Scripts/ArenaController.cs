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
        public GameObject arenaPrefab;

        private List<VBGCharacterController> players = null;
        public List<SpawnPoint> respawns = new List<SpawnPoint>();

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

                if(characterHealth.health <= 0.0f)
                {
                    GameManager.Instance.Death(arenaActive);
                    SoundManager.Instance.PostEvent("Stop_M_BAGARRE_ST", gameObject);
                }

                if (timer <= 0.0f)
                {
                    Victory();
                }
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
            if (allDeads)
            {
                GameManager.Instance.Death(arenaActive);
            }
        }

        void Victory()
        {
            SwitchManager.Instance.SetSwitch("Arena_Victory_10", true);
            characterHealth.SetHealth(100000);
            SoundManager.Instance.PostEvent("Stop_M_BAGARRE_ST", gameObject);
            SwitchManager.Instance.SetSwitch(timerActiveSwitch, false);
        }

        public void ResetArena()
        {
            //Debug.Log("Reset");
            timer = timerMax;
            SwitchManager.Instance.SetSwitch(timerActiveSwitch, false);
            SwitchManager.Instance.SetSwitch("Arena_State_Pre", false);
            SwitchManager.Instance.SetValue("ArenaCollector", 0);
            GameObject old = GameObject.FindGameObjectWithTag("ArenaGameplay");
            GameObject.Destroy(old);
            GameObject newArena = GameObject.Instantiate(arenaPrefab, transform.parent);
            newArena.transform.localPosition = new Vector3();

            int i = 0;
            foreach (VBGCharacterController p in players)
            {
                p.Revive();
                p.transform.position = respawns[i].transform.position;
                i++;
            }

            characterHealth = newArena.transform.Find("Core").GetComponent<CharacterHealth>();

            GameObject[] ennemies = GameObject.FindGameObjectsWithTag(GameManager.Constants.TAG_ENNEMY);
            foreach (GameObject ennemy in ennemies)
            {
                Destroy(ennemy);
            }

            GameManager.Instance.Undeath();
        }
    }
}