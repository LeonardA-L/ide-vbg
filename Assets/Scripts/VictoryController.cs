using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class VictoryController : MonoBehaviour
    {
        public GameObject fxPrefab;
        public void HandleVictory()
        {
            GameObject[] ennemies = GameObject.FindGameObjectsWithTag(GameManager.Constants.TAG_ENNEMY);
            foreach (GameObject ennemy in ennemies)
            {
                GameObject fx = GameObject.Instantiate(fxPrefab);
                fx.transform.position = ennemy.transform.position;
                ennemy.GetComponent<VBGCharacterController>().Damage(-1000);
            }
        }
    }
}