using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SwitchAggregator : MonoBehaviour
    {

        public List<string> andSwitches;
        private List<string> orSwitches;

        private List<string> notSwitches;

        public string outSwitch;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            bool value = true;

            foreach(string s in andSwitches)
            {
                value = value && GameManager.Instance.GetSwitch(s);
            }

            GameManager.Instance.SetSwitch(outSwitch, value);
        }
    }
}