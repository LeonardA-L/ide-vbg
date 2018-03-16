using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class CameraAreaTrigger : MonoBehaviour
    {
        public bool m_aimAtCenter = false;
        [Range(0, 1)]
        public float m_positionSmooth = 0.05f;  // The default value is actually set in the prefab
        [Range(0, 1)]
        public float m_rotationSmooth = 0.05f;

        private void OnTriggerEnter(UnityEngine.Collider _collider)
        {
            if (_collider.gameObject.tag != GameManager.Constants.TAG_PLAYER)
                return;

            Transform cameraSettings = transform.Find("Camera");
            CameraManager.Instance.SetSceneSettings(cameraSettings, m_aimAtCenter, m_positionSmooth, m_rotationSmooth);
        }
    }
}