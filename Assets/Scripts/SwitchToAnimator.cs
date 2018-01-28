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

        [Tooltip("Name of the switch to listen to")]
        public string switchName;

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
            Debug.Assert(switchName != null && switchName != "");
            SwitchManager.Instance.RegisterSwitchListener(switchName, Callback);
        }

        void Callback(bool switchValue)
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
    }
}