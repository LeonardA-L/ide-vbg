using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SwitchToAnimator : MonoBehaviour
    {
        [Tooltip("Name of the boolean parameter to update in the animator")]
        public string boolName;
        [Tooltip("Name of the trigger parameter to update in the animator")]
        public string triggerName;
        [Tooltip("Name of the float parameter to update in the animator")]
        public string floatName;

        [Tooltip("Name of the switch to listen to")]
        public string switchName;

        [Tooltip("Name of the Value to listen to")]
        public string valueName;

        [Tooltip("Animator to update")]
        public Animator animator;

        private bool init = false;

        // Use this for initialization
        void Start()
        {
            
        }

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
            if (valueName != null & valueName != "")
            {
                SwitchManager.Instance.RegisterValueListener(valueName, CallbackValue);
            }
        }

        void CallbackSwitch(bool switchValue)
        {
            if(triggerName != null && triggerName != "")
            {
                animator.SetTrigger(triggerName);
            }

            if (boolName != null && boolName != "")
            {
                animator.SetBool(boolName, switchValue);
            }
        }

        void CallbackValue(float floatValue)
        {
            if (floatName != null && floatName != "")
            {
                animator.SetFloat(floatName, floatValue);
            }
        }
    }
}