using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace vbg
{
    public class ValueToUIText : MonoBehaviour
    {
        [Tooltip("Name of the value to listen to")]
        public string valueName;
        public Text textField;
        private bool init = false;

        // Update is called once per frame
        void Update()
        {
            if (init || SwitchManager.Instance == null)
            {
                return;
            }
            init = true;
            SwitchManager.Instance.RegisterValueListener(valueName, Callback);
        }

        private void Callback(float floatValue)
        {
            textField.text = "" + floatValue;
        }
    }
}