using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class CameraFollowTrigger : MonoBehaviour
    {

        public float m_followDistanceMin = CameraManager.Constants.DEFAULT_FOLLOW_DISTANCE;
        public float m_followDistanceMax = 45;
        public float m_radiusMin = 3;
        public float m_radiusMax = 10;
        [Range(0, 1)]
        public float m_positionSmooth = 0.05f;  // The default value is actually set in the prefab
        [Range(0, 1)]
        public float m_rotationSmooth = 0.05f;

        private void OnTriggerEnter(UnityEngine.Collider _collider)
        {
            if (_collider.gameObject.tag != GameManager.Constants.TAG_PLAYER)
                return;

            Transform cameraSettings = transform.Find("Camera");
            CameraManager.Instance.SetFollowSettings(cameraSettings, m_radiusMin, m_radiusMax, m_followDistanceMin, m_followDistanceMax, m_positionSmooth, m_rotationSmooth);
        }
    }
}