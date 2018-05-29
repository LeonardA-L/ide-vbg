using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

namespace vbg
{
    public class CharacterVibrationController : MonoBehaviour
    {
        private PlayerIndex controllerID;
        private bool set = false;
        private VBGCharacterController cc;
        private float superModeTime = 0.0f;
        public float fadeInTime = 7.0f;
        public float maxFactor = 0.8f;

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

            if(cc.IsSuperMode)
            {
                superModeTime += Time.fixedDeltaTime;
            } else
            {
                superModeTime = 0.0f;
            }

            float factor = Mathf.Clamp(superModeTime / fadeInTime * maxFactor, 0.0f, maxFactor);
            GamePad.SetVibration(controllerID, factor, factor);
        }
    }
}