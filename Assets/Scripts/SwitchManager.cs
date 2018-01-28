using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SwitchManager : MonoBehaviour
    {

        private Dictionary<string, bool> switches;
        public delegate void SwitchCallback(bool switchValue);
        private Dictionary<string, List<SwitchCallback>> switchesPubSub;

        protected static SwitchManager instance;
        public static SwitchManager Instance
        {
            get
            {
                return instance;
            }
        }

        void Start()
        {
            instance = this;

            switches = new Dictionary<string, bool>();
            switchesPubSub = new Dictionary<string, List<SwitchCallback>>();
        }

        public void RegisterSwitchListener(string name, SwitchCallback callback)
        {
            if (!switchesPubSub.ContainsKey(name))
            {
                switchesPubSub.Add(name, new List<SwitchCallback>());
            }
            switchesPubSub[name].Add(callback);
        }

        public bool GetSwitch(string name)
        {
            bool ret = false;
            switches.TryGetValue(name, out ret);
            return ret;
        }

        public void SetSwitch(string name, bool value)
        {
            switches[name] = value;
            List<SwitchCallback> subscribers = null;

            switchesPubSub.TryGetValue(name, out subscribers);

            if (subscribers != null)
            {
                foreach (SwitchCallback sw in subscribers)
                {
                    sw(value);
                }
            }
        }
    }
}