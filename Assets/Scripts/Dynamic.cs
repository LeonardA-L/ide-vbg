using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class Dynamic : MonoBehaviour, IDynamic
    {

        private List<GameEffect> activeGameEffects;
        private Rigidbody rb;

        // Use this for initialization
        void Start()
        {
            activeGameEffects = new List<GameEffect>();
            rb = GetComponent<Rigidbody>();
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
    }
}