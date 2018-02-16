using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/Specific Activator")]
    public class SpecificActivator : GameEffectActivate
    {
        [Tooltip("Only Activator to be able to activate this GameEffect")]
        public Dynamic targetObject;
        public VBGCharacterController targetCharacter;
        private IDynamic target;
        private IDynamic activator;

        void Start()
        {
            target = targetObject ?? (IDynamic)targetCharacter;
        }

        public override bool IsActive(IDynamic idy)
        {
            if (idy == null && activator == null)
            {
                return false;
            }
            if (idy != null)
            {
                activator = idy;
            }
            return idy == target;
        }
    }
}