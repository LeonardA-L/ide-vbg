using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class HacksKawaiiCafe : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButton("r"))
            {
                foreach (VBGCharacterController p in PlayerManager.Instance.GetAllPlayersInGame())
                {
                    p.Revive();
                }
            }
        }
    }
}