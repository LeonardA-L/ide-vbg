using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/OnCollision/NotInvincible")]
    [RequireComponent(typeof(OnCollision))]
    public class NotInvincible : GameEffectActivate
    {
        public override bool IsActive(IDynamic idy)
        {
            VBGCharacterController cc = (idy as VBGCharacterController);
            return cc == null || cc != null && !cc.IsInvincible;
        }
    }
}