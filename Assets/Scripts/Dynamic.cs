using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class Dynamic : MonoBehaviour, IDynamic
    {

        private List<GameEffect> activeGameEffects;
        private Rigidbody rb;
        private CharacterHealth health;

        [Tooltip("A prefab to instantiate when the object dies")]
        public GameObject finishPrefab;

        // Use this for initialization
        void Start()
        {
            activeGameEffects = new List<GameEffect>();
            rb = GetComponent<Rigidbody>();
            health = GetComponent<CharacterHealth>();
        }

        // Update is called once per frame
        void Update()
        {

            // Apply GameEffects
            activeGameEffects.RemoveAll(item => item == null);
            foreach (GameEffect ge in activeGameEffects)
            {
                Vector3 placeholder = new Vector3();
                ge.ProcessOnCollision(this, rb, ref placeholder);
            }

            if (health != null && health.IsDead())
            {
                Die();
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

            Destroy(gameObject);
        }

        public void RegisterGameEffect(GameEffect ge)
        {
            Debug.Log("Dynamic Register " + gameObject.name);
            activeGameEffects.Add(ge);
        }

        public void UnRegisterGameEffect(GameEffect ge)
        {
            Debug.Log("Dynamic UnRegister " + gameObject.name);
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
    }
}