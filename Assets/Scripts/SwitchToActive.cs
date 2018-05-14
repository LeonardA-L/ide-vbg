using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SwitchToActive : MonoBehaviour
    {
        public GameObject target;
        public string switchName;
        public bool revert = false;
        private bool init = false;

        void Update()
        {
            if (init || SwitchManager.Instance == null)
            {
                return;
            }
            init = true;
            if (switchName != null & switchName != "")
            {
                SwitchManager.Instance.RegisterSwitchListener(switchName, CallbackSwitch);
            }
        }

        void CallbackSwitch(bool switchValue)
        {
            target.SetActive(revert ? !switchValue : switchValue);
        }
    }
}