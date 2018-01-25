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

        public override bool AfterProcess() {
            times--;
            return times == 0;
        }
    }
}