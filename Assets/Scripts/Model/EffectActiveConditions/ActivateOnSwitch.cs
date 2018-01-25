using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class ActivateOnSwitch : GameEffectActivate
    {
        [Tooltip("Name of the switch to listen to")]
        public string switchName;
        private bool isActive = false;
        private bool init = false;

        void Update()
        {
            if (init || GameManager.Instance == null)
            {
                return;
            }
            init = true;
            GameManager.Instance.RegisterSwitchListener(switchName, Callback);
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