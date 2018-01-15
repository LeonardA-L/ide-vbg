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
            health = VBGCharacterController.Constants.CHARACTER_START_HEALTH;
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
            health += intensity;
            health = ClampHealth();
        }

        public void Heal(float intensity)
        {
            health += intensity;
            health = ClampHealth();
        }

        public float GetHealth()
        {
            return health;
        }

        public bool IsDead()
        {
            return health <= 0.0f;
        }

        private float ClampHealth()
        {
            return Mathf.Clamp(health, 0.0f, 200.0f);
        }
    }
}