using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/Activate On Weapon")]
    public class ActivateWeapon : GameEffectActivate
    {
        private VBGCharacterController owner;

        void Start()
        {
            owner = GetComponent<GameEffect>().GetOwner();
            Debug.Assert(owner);
        }

        public override bool IsActive(IDynamic idy) {
            return owner.WeaponIsActive;
        }
    }
}