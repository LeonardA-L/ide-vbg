using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class DelayFinish : GameEffectExit
    {
        [Tooltip("Delay before the effect is killed, in s")]
        public float delay = 10;

        public override bool AfterUpdate() {
            delay -= Time.deltaTime;
            return delay <= 0;
        }
    }
}