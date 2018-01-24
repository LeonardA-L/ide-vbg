using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SwitchToAnimator : MonoBehaviour
    {
        public string boolName;
        public string triggerName;

        public string switchName;

        public Animator animator;

        private bool init = false;

        // Use this for initialization
        void Start()
        {
            
        }

        void Update()
        {
            if (init || GameManager.Instance == null)
            {
                return;
            }
            init = true;
            Debug.Assert(switchName != null && switchName != "");
            GameManager.Instance.RegisterSwitchListener(switchName, Callback);
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