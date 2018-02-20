using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SwitchNOTGate : MonoBehaviour
    {

        public string inSwitch;
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

            SwitchManager.Instance.RegisterSwitchListener(inSwitch, Callback);
        }
        
        void Callback(bool switchValue)
        {
            SwitchManager.Instance.SetSwitch(outSwitch, !switchValue);
        }
    }
}