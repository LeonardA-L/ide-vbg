using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/OnCollision/Only Creatures")]
    [RequireComponent(typeof(OnCollision))]
    public class OnlyCreatures : GameEffectActivate
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
            VBGCharacterController cc = idy as VBGCharacterController;
            return cc != null;
        }
    }
}