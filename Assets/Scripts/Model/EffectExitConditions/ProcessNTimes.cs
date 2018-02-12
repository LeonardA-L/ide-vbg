using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ExitConditions/Process N Times")]
    public class ProcessNTimes : GameEffectExit
    {
        [Tooltip("Number of times the GameEffect should process begore being killed")]
        public int times = 1;
        public int currentTimes = 1;

        public override bool AfterProcess() {
            currentTimes--;
            return currentTimes == 0;
        }

        public override void Reset()
        {
            currentTimes = times;
        }
    }
}