using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/OnCollision/On Collision")]
    public class OnCollision : GameEffectActivate
    {
        public override bool IsActive(IDynamic idy)
        {
            return idy != null;
        }
    }
}