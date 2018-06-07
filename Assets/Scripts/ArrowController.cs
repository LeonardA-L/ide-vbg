using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg {
    public class ArrowController : MonoBehaviour {
        public bool lerpDown = false;


        // Use this for initialization
        void Start() {
            VBGCharacterController owner = GetComponent<GameEffect>().GetOwner();
            ArrowNavigator nav = owner.transform.Find("ArrowNavigator").GetComponent<ArrowNavigator>();
            Debug.Assert(nav != null, "No navigator");

            Transform target = nav.GetBestTarget();
            //Debug.Log(target);
            if (target == null)
                return;

            Vector3 segment = target.position - transform.position;
            segment.y = 0.0f;
            segment.Normalize();
            //Debug.DrawLine(transform.position, transform.position + segment * 10.0f, Color.red, 2.0f);
            ///Debug.Log("A a " + transform.forward);
            transform.forward = segment;
            //Debug.Log("A b " + transform.forward);
        }

        private void Update()
        {
            if(lerpDown)
            {
                transform.forward = Vector3.Lerp(transform.forward, new Vector3(transform.forward.x, -0.5f, transform.forward.z), 0.01f);
            }
        }
    }
}