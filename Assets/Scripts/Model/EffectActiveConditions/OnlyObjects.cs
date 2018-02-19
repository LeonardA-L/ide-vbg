using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/Only Objects")]
    public class OnlyObjects : GameEffectActivate
    {
        void Start()
        {
        }

        public override bool IsActive(IDynamic idy)
        {
            if (idy == null)
            {
                return false;
            }
            Dynamic dy = idy as Dynamic;
            return dy != null;
        }
    }
}