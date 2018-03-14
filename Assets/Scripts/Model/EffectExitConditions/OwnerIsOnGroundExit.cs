using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ExitConditions/Owner Is On Ground")]
    public class OwnerIsOnGroundExit : GameEffectExit
    {
        private VBGCharacterController owner;

        void Start()
        {
            owner = GetComponent<GameEffect>().GetOwner();
            Debug.Assert(owner);
        }

        public override bool AfterProcess()
        {
            return owner.isGrounded;
        }
    }
}