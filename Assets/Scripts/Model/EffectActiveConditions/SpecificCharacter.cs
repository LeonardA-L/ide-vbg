using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SpecificCharacter : GameEffectActivate
    {
        public VBGCharacterController.Character target;
        public VBGCharacterController activator;

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