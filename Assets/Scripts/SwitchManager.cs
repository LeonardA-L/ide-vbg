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

        public bool debugSwitches = false;
        public bool debugValues = false;

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

        void OnGUI()
        {
            DisplayDebug();
        }

        private void DisplayDebug()
        {
            if (!Debug.isDebugBuild)
            {
                return;
            }

            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = h * 2 / 100
            };
            style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            if (debugSwitches)
            {
                Rect rect = new Rect(0, 0, w, h * 2 / 100);
                string text = "";

                foreach (KeyValuePair<string, bool> entry in switches)
                {
                    text += entry.Key + ": " + entry.Value + "\n";
                }

                GUI.Label(rect, text, style);
            }
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