using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace vbg
{
    public class VSFightingMap : MonoBehaviour
    {
        private Dictionary<VBGCharacterController, GameObject> playerUIs = new Dictionary<VBGCharacterController, GameObject>();
        public List<GameObject> HUDs = new List<GameObject>();
        private int currentIdx = 0;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            foreach(VBGCharacterController player in PlayerManager.Instance.GetAllPlayersInGame())
            {
                Transform img = playerUIs[player].transform.Find("Life").transform;
                img.localScale = Vector3.Lerp(img.localScale, new Vector3(player.GetHealth().GetHealth() / player.GetHealth().MaxHealth, img.localScale.y, img.localScale.z), 0.3f);

            }
        }

        public void OnDeath(VBGCharacterController player)
        {
            Debug.Log("??");
            Text deathCount = playerUIs[player].transform.Find("DeathCount").GetComponent<Text>();
            int num = int.Parse(deathCount.text);
            num++;
            deathCount.text = "" + num;
        }

        public void NewPlayer(VBGCharacterController player)
        {
            playerUIs.Add(player, HUDs[currentIdx]);
            Text deathCount = playerUIs[player].transform.Find("DeathCount").GetComponent<Text>();
            deathCount.text = "0";
            Transform img = playerUIs[player].transform.Find("Life").transform;
            img.gameObject.SetActive(true);
            currentIdx++;
        }
    }
}