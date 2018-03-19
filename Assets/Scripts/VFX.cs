using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class VFX : MonoBehaviour
    {

        private List<ParticleSystem> systems = new List<ParticleSystem>();

        private bool hasEnded = false;
        private Transform follow = null;
        private Vector3 followOffset;
        private bool followRotation = false;

        public float destroyTimeout = 5.0f;

        private void FindParticles(Transform _tr)
        {
            ParticleSystem system = _tr.GetComponent<ParticleSystem>();
            if (system != null)
            {
                systems.Add(system);
            }

            foreach(Transform child in _tr)
            {
                FindParticles(child);
            }
        }

        // Use this for initialization
        void Start()
        {
            FindParticles(transform);
        }

        public void SetFXData(bool _followRotation = false, Transform _follow = null)
        {
            follow = _follow;
            followRotation = _followRotation;

            if (follow != null)
            {
                followOffset = transform.position - follow.position;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(hasEnded)
            {
                destroyTimeout -= Time.deltaTime;
                if(destroyTimeout <= 0.0f)
                {
                    Destroy(gameObject);
                }
            }

            if(follow != null)
            {
                transform.position = follow.position + followOffset;

                if(followRotation)
                {
                    transform.rotation = follow.rotation;
                }
            }
        }

        public void End()
        {
            hasEnded = true;
            foreach(ParticleSystem system in systems)
            {
                system.Stop();
            }
        }
    }
}