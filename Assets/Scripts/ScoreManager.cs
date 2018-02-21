using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class ScoreManager : MonoBehaviour
    {
        protected static ScoreManager m_instance;
        public static ScoreManager Instance
        {
            get
            {
                return m_instance;
            }
        }

        private float m_score = 0.0f;
        private float m_pending = 0.0f;

        void Start()
        {
            m_instance = this;
        }

        public void AddScore(float _diff, bool _hold = false)
        {
            m_pending += _diff;

            if (!_hold)
            {
                m_score += m_pending;
                m_pending = 0.0f;
            }
        }
    }
}