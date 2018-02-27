using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof(GameEffect))]
    public class GameEffectActivate : MonoBehaviour
    {
        protected GameEffect gameEffect;

        void Start()
        {
            gameEffect = GetComponent<GameEffect>();
        }

        public virtual bool IsActive(IDynamic idy)
        {
            return false;
        }

        public virtual void Reset() { }
    }
}