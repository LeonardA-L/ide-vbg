using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SwitchANDGate : MonoBehaviour
    {

        public List<string> andSwitches;

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

            foreach (string s in andSwitches)
            {
                SwitchManager.Instance.RegisterSwitchListener(s, Callback);
            }
        }
        
        void Callback(bool switchValue)
        {
            bool value = true;

            foreach(string s in andSwitches)
            {
                value = value && SwitchManager.Instance.GetSwitch(s);
            }

            SwitchManager.Instance.SetSwitch(outSwitch, value);
        }
    }
}