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

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(switchName == null || switchName == "")
            {
                return;
            }

            bool switchValue = GameManager.Instance.GetSwitch(switchName);

            if(switchValue)
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
}