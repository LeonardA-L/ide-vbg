using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class ActivateWeapon : GameEffectActivate
    {
        private VBGCharacterController owner;

        void Start()
        {
            owner = GetComponent<GameEffect>().GetOwner();
            Debug.Assert(owner);
        }

        public override bool IsActive() {
            return owner.WeaponIsActive;
        }
    }
}