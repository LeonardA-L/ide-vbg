using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace vbg
{
    public class ValueRatioToScale : MonoBehaviour
    {

        [Tooltip("Name of the value to listen to")]
        public string valueName;
        public Image img;
        public float lerp = 0.3f;
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
            img.transform.localScale = Vector3.Lerp(img.transform.localScale, new Vector3(floatValue, img.transform.localScale.y, img.transform.localScale.z), lerp);
        }
    }
}