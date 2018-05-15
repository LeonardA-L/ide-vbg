using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg {
    public class FaceCamera : MonoBehaviour {
        public RectTransform to2D;

        // Update is called once per frame
        void Update() {
            to2D.forward = Camera.main.transform.forward;
        }
    }
}