using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class CopyRotation3DTo2D : MonoBehaviour
    {
        public Transform from3D;
        public RectTransform to2D;

        // Update is called once per frame
        void Update()
        {
            to2D.localRotation = Quaternion.Euler(0, 0.0f, Quaternion.ToEulerAngles(from3D.rotation).y * Mathf.Rad2Deg);
        }
    }
}