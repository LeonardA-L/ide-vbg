using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class FinishOnAnimatorState : GameEffectExit
    {
        public Animator animator;
        public string state;

        public override bool AfterUpdate() {
            if(animator.GetCurrentAnimatorStateInfo(0).IsName(state))
            {
                return true;
            }
            return false;
        }
    }
}