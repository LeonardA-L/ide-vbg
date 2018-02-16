using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/Owner Is OnGround")]
    public class OwnerIsOnGround : GameEffectActivate
    {
        private VBGCharacterController owner;

        void Start()
        {
            owner = GetComponent<GameEffect>().GetOwner();
            Debug.Assert(owner);
        }

        public override bool IsActive(VBGCharacterController cc)
        {
            return owner.isGrounded;
        }
    }
}