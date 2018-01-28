using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/Activate On Switch")]
    public class ActivateOnSwitch : GameEffectActivate
    {
        [Tooltip("Name of the switch to listen to")]
        public string switchName;
        private bool isActive = false;
        private bool init = false;

        void Update()
        {
            if (init || SwitchManager.Instance == null)
            {
                return;
            }
            init = true;
            SwitchManager.Instance.RegisterSwitchListener(switchName, Callback);
        }

        public override bool IsActive(VBGCharacterController cc)
        {
            return isActive;
        }

        private void Callback(bool switchValue)
        {
            isActive = switchValue;
        }
    }
}