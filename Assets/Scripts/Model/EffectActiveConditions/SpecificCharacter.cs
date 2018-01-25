using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/Specific Character")]
    public class SpecificCharacter : GameEffectActivate
    {
        [Tooltip("Only Character to be able to activate this GameEffect")]
        public VBGCharacterController.Character target;
        private VBGCharacterController activator;

        void Start()
        {
        }

        public override bool IsActive(VBGCharacterController cc) {
            if(cc == null && activator == null)
            {
                return false;
            }
            if(cc != null)
            {
                activator = cc;
            }
            return activator.character == target;
        }
    }
}