using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

namespace vbg
{
    public class CharacterVibrationController : MonoBehaviour
    {
        private static readonly float GLOBAL_FACTOR = 0.6f;

        private PlayerIndex controllerID;
        private bool set = false;
        private VBGCharacterController cc;
        private float superModeTime = 0.0f;
        public float fadeInTime = 7.0f;
        public float maxFactor = 0.8f;

        private float force = 0.0f;
        private float forceGoal = 0.0f;
        private float duration = 0.0f;
        private float lerpFactor = 1.0f;

        public void SetController(int _controllerID)
        {
            controllerID = (PlayerIndex)(_controllerID -1);
            set = true;
        }

        void Start()
        {
            cc = GetComponent<VBGCharacterController>();
        }

        void FixedUpdate()
        {
            if (!set)
                return;

            if(cc.IsDead())
            {
                GamePad.SetVibration(controllerID, 0, 0);
                return;
            }

            
            if(duration > 0)
            {
                duration -= Time.fixedDeltaTime;
                force = Mathf.Lerp(force, forceGoal, lerpFactor);
                if(duration <= 0)
                {
                    force = 0;
                }
                duration = Mathf.Max(duration, 0);
                GamePad.SetVibration(controllerID, force, force);
            }
        }

        public void SetVibration(float _force, float _duration, float _lerpFactor = 1.0f)
        {
            forceGoal = Mathf.Max(_force, forceGoal);
            duration = Mathf.Max(duration, _duration);
            lerpFactor = _lerpFactor;
        }
    }
}