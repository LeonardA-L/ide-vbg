using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg {
    public class HackTeleportPuzzleCrane : MonoBehaviour {
        public List<Transform> hotspots = new List<Transform>();


        public void HackTeleport()
        {
            List<VBGCharacterController> players = PlayerManager.Instance.GetAllPlayersInGame();
            int i = 0;
            foreach(VBGCharacterController p in players)
            {
                p.transform.position = hotspots[i].position;
                i++;
            }
        }
    }
}