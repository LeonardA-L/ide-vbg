using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace vbg
{
    public class ValueRatioToRadialFill : MonoBehaviour
    {

        [Tooltip("Name of the value to listen to")]
        public string valueName;
        public Image img;
        public float factor = 1.0f;
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
            img.fillAmount = Mathf.Lerp(img.fillAmount, floatValue * factor, 0.9f);
        }
    }
}