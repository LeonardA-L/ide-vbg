using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class ArrowNavigator : MonoBehaviour
    {

        public List<Transform> m_ennemies = new List<Transform>();
        public List<Transform> m_objects = new List<Transform>();

        // Use this for initialization
        void Start()
        {

        }

        private void OnTriggerEnter(UnityEngine.Collider collision)
        {
            Transform tr = collision.transform;
            //Debug.Log("In " + tr.name + " " + tr.tag);
            if (tr.gameObject.layer == GameManager.Constants.LAYER_ARROW)
                return;
            if (tr.gameObject.tag == GameManager.Constants.TAG_ENNEMY)
            {
                m_ennemies.Add(tr);
            } else if (tr.tag == GameManager.Constants.TAG_DYNAMIC || tr.gameObject.tag == GameManager.Constants.TAG_GAMEEFFECT)
            {
                m_objects.Add(tr);
            }
        }

        private void OnTriggerExit(UnityEngine.Collider collision)
        {
            Transform tr = collision.transform;
            //Debug.Log("Out " + tr.name);
            if (tr.gameObject.layer == GameManager.Constants.LAYER_ARROW)
                return;

            if (tr.tag == GameManager.Constants.TAG_ENNEMY)
            {
                m_ennemies.Remove(tr);
            }
            else if (tr.gameObject.tag == GameManager.Constants.TAG_DYNAMIC || tr.gameObject.tag == GameManager.Constants.TAG_GAMEEFFECT)
            {
                m_objects.Remove(tr);
            }
        }

        private Transform GetBestFromList(List<Transform> list)
        {
            float minDist = float.MaxValue;
            Transform best = list[0];

            foreach (Transform tr in list)
            {
                if ((tr.position - transform.parent.position).magnitude < minDist)
                {
                    best = tr;
                    minDist = (tr.position - transform.parent.position).magnitude;
                }
            }
            Debug.Log(best);
            return best;
        }

        public Transform GetBestTarget()
        {
            Debug.Log("Nav " + name);
            m_ennemies.RemoveAll(item => item == null);
            Debug.Log("Ennemies " + m_ennemies.Count);
            if (m_ennemies.Count > 0)
            {
                return GetBestFromList(m_ennemies);
            }
            m_objects.RemoveAll(item => item == null);
            Debug.Log("Objects " + m_objects.Count);
            if (m_objects.Count > 0)
            {
                return GetBestFromList(m_objects);
            }
            Debug.Log("None");

            return null;
        }
    }
}