using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/OnCollision/NotOwner")]
    [RequireComponent(typeof(OnCollision))]
    public class NotOwner : GameEffectActivate
    {
        public override bool IsActive(IDynamic idy)
        {
            return idy != null && (idy as VBGCharacterController) != gameEffect.GetOwner();
        }
    }
}