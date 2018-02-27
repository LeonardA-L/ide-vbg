using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/OnCollision/Specific Character")]
    [RequireComponent(typeof(OnCollision))]
    public class SpecificCharacter : GameEffectActivate
    {
        [Tooltip("Only Character to be able to activate this GameEffect")]
        public VBGCharacterController.Character target;
        private VBGCharacterController activator;

        void Start()
        {
        }

        public override bool IsActive(IDynamic idy)
        {
            if (idy == null && activator == null)
            {
                return false;
            }
            if (idy != null)
            {
                activator = (VBGCharacterController) idy;
            }
            return activator.character == target;
        }
    }
}