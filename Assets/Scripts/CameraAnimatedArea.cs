using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class CameraAnimatedArea : MonoBehaviour
    {
        public Transform hotspotStart;
        public Transform hotspotEnd;
        public Animator cameraAnimator;

        private void OnTriggerEnter(UnityEngine.Collider _collider)
        {
            if (_collider.gameObject.tag != GameManager.Constants.TAG_PLAYER)
                return;

            Transform cameraSettings = transform.Find("Camera");
            CameraManager.Instance.SetAnimatedSettings(cameraSettings, hotspotStart, hotspotEnd, cameraAnimator);
        }
    }
}