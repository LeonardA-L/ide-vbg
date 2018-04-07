using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class HackSwitchOnInput : MonoBehaviour
    {

        public string keyName = "";
        public string switchName = "";

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButton(keyName))
            {
                SwitchManager.Instance.SetSwitch(switchName, true);
            }
        }
    }
}
