using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class CharacterHealth : MonoBehaviour
    {

        public float health;
        public float threshold = 0.0f;

        // Use this for initialization
        void Start()
        {
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
            if(Mathf.Abs(intensity) < threshold)
            {
                return;
            }
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

        public void SetHealth(float _health)
        {
            health = _health;
            health = ClampHealth();
        }

        public bool IsDead()
        {
            return health <= 0.0f;
        }

        private float ClampHealth()
        {
            return Mathf.Clamp(health, 0.0f, 100.0f);
        }
    }
}