using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class ChildCollider : MonoBehaviour
    {
        private List<VBGCharacterController> m_inRange;

        // Use this for initialization
        void Start()
        {
            m_inRange = new List<VBGCharacterController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            VBGCharacterController cc = other.gameObject.GetComponent<VBGCharacterController>();

            if (cc != null && cc.IsPlayer)
            {
                m_inRange.Add(cc);
            }
        }

        private void OnTriggerExit(UnityEngine.Collider other)
        {
            VBGCharacterController cc = other.gameObject.GetComponent<VBGCharacterController>();

            if (cc != null)
            {
                m_inRange.Remove(cc);
            }
        }

        public List<VBGCharacterController> GetCharactersInRange()
        {
            return m_inRange;
        }
    }
}