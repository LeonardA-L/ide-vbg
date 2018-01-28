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

        private Dictionary<string, float> floatValues;
        public delegate void FloatCallback(float floatValue);
        private Dictionary<string, List<FloatCallback>> floatValuesPubSub;

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

            floatValues = new Dictionary<string, float>();
            floatValuesPubSub = new Dictionary<string, List<FloatCallback>>();
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
            

            if (debugSwitches)
            {
                GUIStyle style = new GUIStyle
                {
                    alignment = TextAnchor.UpperLeft,
                    fontSize = h * 2 / 100
                };
                style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                Rect rect = new Rect(0, 0, w, h * 2 / 100);
                string text = "";

                foreach (KeyValuePair<string, bool> entry in switches)
                {
                    text += entry.Key + ": " + entry.Value + "\n";
                }

                GUI.Label(rect, text, style);
            }

            if (debugValues)
            {
                GUIStyle style = new GUIStyle
                {
                    alignment = TextAnchor.UpperRight,
                    fontSize = h * 2 / 100
                };
                style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                Rect rect = new Rect(0, 0, w, h * 2 / 100);
                string text = "";

                foreach (KeyValuePair<string, float> entry in floatValues)
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

        public void RegisterValueListener(string name, FloatCallback callback)
        {
            if (!floatValuesPubSub.ContainsKey(name))
            {
                floatValuesPubSub.Add(name, new List<FloatCallback>());
            }
            floatValuesPubSub[name].Add(callback);
        }

        public float? GetValue(string name)
        {

            if(!floatValues.ContainsKey(name))
            {
                return null;
            }

            float ret;
            floatValues.TryGetValue(name, out ret);
            return ret;
        }

        public void SetValue(string name, float value)
        {
            floatValues[name] = value;
            List<FloatCallback> subscribers = null;

            floatValuesPubSub.TryGetValue(name, out subscribers);

            if (subscribers != null)
            {
                foreach (FloatCallback vc in subscribers)
                {
                    vc(value);
                }
            }
        }
    }
}