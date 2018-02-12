using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/ActivateConditions/Delay Activate")]
    public class DelayActivate : GameEffectActivate
    {
        [Tooltip("Delay before the effect is activated, in s")]
        public float delay = 10;
        public float currentDelay = 10;
        [Tooltip("Optional. Name of a switch to wait on before starting the timer")]
        public string switchStart;
        private bool isTimerActive = false;
        private bool init = false;

        private void Start()
        {
            Reset();
            init = isTimerActive;
        }

        void Update()
        {
            if (isTimerActive)
            {
                currentDelay -= Time.deltaTime;
            }

            if (init || SwitchManager.Instance == null)
            {
                return;
            }
            init = true;
            SwitchManager.Instance.RegisterSwitchListener(switchStart, Callback);

        }

        public override bool IsActive(VBGCharacterController cc)
        {
            return currentDelay <= 0;
        }

        private void Callback(bool switchValue)
        {
            isTimerActive = switchValue;
        }

        public override void Reset()
        {
            isTimerActive = !(switchStart != null && switchStart != "");
            currentDelay = delay;
        }
    }
}