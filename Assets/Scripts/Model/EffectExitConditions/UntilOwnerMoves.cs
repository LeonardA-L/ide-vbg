using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ExitConditions/Until Owner Moves")]
    public class UntilOwnerMoves : GameEffectExit
    {
        private Vector3 initPosition;
        private VBGCharacterController owner;
        private bool isInit = false;

        public override bool AfterUpdate() {
            if(!isInit)
            {
                GameEffect ge = GetComponent<GameEffect>();
                owner = ge.GetOwner();
                if (owner == null)
                {
                    return false;
                }
                isInit = true;
                initPosition = owner.transform.position;
                //Debug.Log(owner.name);
            }

            if((owner.transform.position - initPosition).magnitude > 0.05f)
            {
                return true;
            }
            return false;
        }
    }
}