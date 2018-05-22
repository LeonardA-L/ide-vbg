using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ExitConditions/Delay Finish")]
    public class DelayFinish : GameEffectExit
    {
        [Tooltip("Delay before the effect is killed, in s")]
        public float delay = 10;
        public float currentDelay = 10;

        private void Update()
        {
            currentDelay -= Time.deltaTime;
        }

        public override bool AfterProcess() {
            return currentDelay <= 0;
        }

        public override void Reset()
        {
            currentDelay = delay;
        }
    }
}