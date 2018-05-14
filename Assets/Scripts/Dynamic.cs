using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class Dynamic : MonoBehaviour, IDynamic
    {

        private List<GameEffect> activeGameEffects = new List<GameEffect>();
        private Rigidbody rb;
        private CharacterHealth health;

        public float minForceToEnableRigidbody = 0.0f;
        public string soundOnEnable;
        public float chaotic = 0.0f;
        public float radius = 1.0f;
        private bool soundActive = false;
        public string soundActiveEventPlay;
        public string soundActiveEventStop;
        private List<UnityEngine.Collider> activeColliders = new List<UnityEngine.Collider>();
        private Vector3 lastPosition;

        [Tooltip("A prefab to instantiate when the object dies")]
        public GameObject finishPrefab;

        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            health = GetComponent<CharacterHealth>();
            lastPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {

            // Apply GameEffects
            activeGameEffects.RemoveAll(item => item == null);
            foreach (GameEffect ge in activeGameEffects)
            {
                if (minForceToEnableRigidbody > 0.0f && ge.pushForce.active && ge.pushForce.pushForceNorm > minForceToEnableRigidbody)
                {
                    rb.isKinematic = false;
                    if(null != soundOnEnable && soundOnEnable != "")
                    {
                        SoundManager.Instance.PostEvent(soundOnEnable, gameObject);
                        soundOnEnable = null;
                    }
                }

                Vector3 placeholder = new Vector3();
                ge.ProcessOnCollision(this, rb, ref placeholder);
            }

            if (health != null && health.IsDead())
            {
                Die();
            }

            float movingDiff = (transform.position - lastPosition).magnitude;
            float movingThr = 0.02f;
            
            if (movingDiff > movingThr)
            {
                // Add sound
                if(soundActiveEventPlay != null && soundActiveEventPlay != "" && !soundActive && activeColliders.Count > 0)
                {
                    soundActive = true;
                    SoundManager.Instance.PostEvent(soundActiveEventPlay, gameObject);
                }
                // Add chaos
                if (chaotic != 0.0f)
                {
                    float chaos = chaotic * rb.mass / GameManager.Constants.CHAOS_MASS_REFERENCE * movingDiff;
                    GameManager.Instance.AddChaos(chaos);
                }
            }
            lastPosition = transform.position;

            // Remove sound
            if (soundActiveEventStop != null && soundActiveEventStop != "" && soundActive && (activeColliders.Count == 0 || movingDiff < 0.003f))
            {
                soundActive = false;
                SoundManager.Instance.PostEvent(soundActiveEventStop, gameObject);
            }
        }

        private void Die()
        {
            if (finishPrefab != null)
            {
                GameObject finishObject = GameObject.Instantiate(finishPrefab);
                finishObject.transform.position = transform.position;
                finishObject.transform.rotation = transform.rotation;
            }

            if(health.chaosOnDeath != 0.0f)
            {
                GameManager.Instance.AddChaos(health.chaosOnDeath);
            }

            Destroy(gameObject);
        }

        public void RegisterGameEffect(GameEffect ge)
        {
            //Debug.Log("Dynamic Register " + ge + " " + gameObject.name + " " + activeGameEffects.Count);
            activeGameEffects.Add(ge);
        }

        public void UnRegisterGameEffect(GameEffect ge)
        {
            //Debug.Log("Dynamic UnRegister " + gameObject.name);
            activeGameEffects.Remove(ge);
        }

        public void Damage(float intensity)
        {
            if (health == null || health.IsDead())
            {
                return;
            }
            health.Damage(intensity);
        }

        public void Heal(float intensity)
        {
            if (health == null || health.IsDead())
            {
                return;
            }
            health.Damage(intensity);
        }

        public float GetRadius()
        {
            return radius;
        }

        private void OnCollisionEnter(Collision collision)
        {
            UnityEngine.Collider col = collision.collider;
            if(!activeColliders.Contains(col))
            {
                activeColliders.Add(col);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            UnityEngine.Collider col = collision.collider;
            if (activeColliders.Contains(col))
            {
                activeColliders.Remove(col);
            }
        }
    }
}