﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SwitchORGate : MonoBehaviour
    {

        public List<string> orSwitches;

        public string outSwitch;

        private bool init = false;

        // Use this for initialization
        void Start()
        {

        }

        private void Update()
        {
            if (init || SwitchManager.Instance == null)
            {
                return;
            }
            init = true;

            foreach (string s in orSwitches)
            {
                SwitchManager.Instance.RegisterSwitchListener(s, Callback);
            }
        }

        void Callback(bool switchValue)
        {
            bool value = false;

            foreach (string s in orSwitches)
            {
                value = value || SwitchManager.Instance.GetSwitch(s);
            }

            SwitchManager.Instance.SetSwitch(outSwitch, value);
        }
    }
}