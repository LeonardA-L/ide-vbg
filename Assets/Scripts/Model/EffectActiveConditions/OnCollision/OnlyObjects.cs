using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/OnCollision/Only Objects")]
    [RequireComponent(typeof(OnCollision))]
    public class OnlyObjects : GameEffectActivate
    {
        void Start()
        {
        }

        public override bool IsActive(IDynamic idy)
        {
            if (idy == null)
            {
                return true;
            }
            Dynamic dy = idy as Dynamic;
            return dy != null;
        }
    }
}