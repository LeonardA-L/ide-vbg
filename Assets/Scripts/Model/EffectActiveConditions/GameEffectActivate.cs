using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof(GameEffect))]
    public class GameEffectActivate : MonoBehaviour
    {

        void Start()
        {
            // TODO get GameEffect component ?
        }

        public virtual bool IsActive(IDynamic idy)
        {
            return false;
        }

        public virtual void Reset() { }
    }
}