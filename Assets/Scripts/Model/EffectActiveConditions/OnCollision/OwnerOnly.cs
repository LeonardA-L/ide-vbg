using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/OnCollision/OwnerOnly")]
    [RequireComponent(typeof(OnCollision))]
    public class OwnerOnly : GameEffectActivate
    {
        public override bool IsActive(IDynamic idy)
        {
            return idy != null && idy as VBGCharacterController == gameEffect.GetOwner();
        }
    }
}