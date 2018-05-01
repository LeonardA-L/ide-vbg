using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class CharacterHealth : MonoBehaviour
    {

        public float health;
        public float threshold = 0.0f;
        public float impactThreshold = -1.0f;
        public float impactMultiplier = 1.0f;
        public float chaosOnDeath = 0.0f;
        private Vector3 lastPosition;

        // Use this for initialization
        void Start()
        {
            // TODO set start health from stats / armor / whatever
        }

        // Update is called once per frame
        void Update()
        {
            lastPosition = transform.position;
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

        private void OnCollisionEnter(Collision collision)
        {
            if(impactThreshold <= 0.0f)
            {
                return;
            }

            Vector3 velocity = transform.position - lastPosition;
            velocity /= Time.deltaTime;
            
            Vector3 normal = collision.contacts[0].normal;
            
            float impact = Mathf.Abs(Mathf.Min(Vector3.Dot(velocity, normal), 0.0f));
            impact -= impactThreshold;

            if(impact > 0.0f)
            {
                Damage(-impact * impactMultiplier);
            }
        }
    }
}