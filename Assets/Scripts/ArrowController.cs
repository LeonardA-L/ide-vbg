using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg {
    public class ArrowController : MonoBehaviour {

        // Use this for initialization
        void Start() {
            VBGCharacterController owner = GetComponent<GameEffect>().GetOwner();
            ArrowNavigator nav = owner.transform.Find("ArrowNavigator").GetComponent<ArrowNavigator>();
            Debug.Assert(nav != null, "No navigator");

            Transform target = nav.GetBestTarget();
            Debug.Log(target);
            if (target == null)
                return;

            Vector3 segment = target.position - transform.position;
            segment.y = 0.0f;
            Debug.Log("A a " + transform.forward);
            transform.forward = segment;
            Debug.Log("A b " + transform.forward);
        }
    }
}