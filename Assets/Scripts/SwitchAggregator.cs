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

        private bool init = false;

        // Use this for initialization
        void Start()
        {

        }

        private void Update()
        {
            if (init || GameManager.Instance == null)
            {
                return;
            }
            init = true;

            foreach (string s in andSwitches)
            {
                GameManager.Instance.RegisterSwitchListener(s, Callback);
            }
        }
        
        void Callback(bool switchValue)
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