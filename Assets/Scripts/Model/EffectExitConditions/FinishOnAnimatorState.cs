using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ExitConditions/Finish On Animator State")]
    public class FinishOnAnimatorState : GameEffectExit
    {
        [Tooltip("Animator to listen to")]
        public Animator animator;
        [Tooltip("Name of the state in which the game effect finishes")]
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