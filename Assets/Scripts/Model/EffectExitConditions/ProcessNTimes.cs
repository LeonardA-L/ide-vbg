using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class ProcessNTimes : GameEffectExit
    {
        public int times = 1;

        public override bool AfterProcess() {
            times--;
            return times == 0;
        }
    }
}