using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof(GameEffect))]
    public class GameEffectExit : MonoBehaviour
    {

        void Start()
        {
            // TODO get GameEffect component ?
        }

        public virtual bool AfterProcess()
        {
            return false;
        }

        public virtual bool AfterUpdate()
        {
            return false;
        }

        public virtual void Reset() {}
    }
}