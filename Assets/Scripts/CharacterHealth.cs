using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class CharacterHealth : MonoBehaviour
    {

        public float health;

        // Use this for initialization
        void Start()
        {
            health = GameManager.Constants.CHARACTER_START_HEALTH;
            // TODO set start health from stats / armor / whatever
        }

        // Update is called once per frame
        void Update()
        {
            // Poison ?
        }

        public void Damage(float intensity)
        {
            // TODO calculate actual blow with stats/def/armor/...
            health -= intensity;
        }

        public void Heal(float intensity)
        {
            health += intensity;
        }

        public float GetHealth()
        {
            return health;
        }
    }
}