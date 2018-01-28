using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/Activate On Value")]
    public class ActivateOnValue : GameEffectActivate
    {
        [Tooltip("Name of the value to listen to")]
        public string valueName;
        public float activeRangeMin;
        public float activeRangeMax;
        private bool isActive = false;
        private bool init = false;

        void Update()
        {
            if (init || SwitchManager.Instance == null)
            {
                return;
            }
            init = true;
            SwitchManager.Instance.RegisterValueListener(valueName, Callback);
        }

        public override bool IsActive(VBGCharacterController cc)
        {
            return isActive;
        }

        private void Callback(float floatValue)
        {
            isActive = floatValue <= activeRangeMax && floatValue >= activeRangeMin;
        }
    }
}