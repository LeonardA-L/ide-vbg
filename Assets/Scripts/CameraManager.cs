using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class CameraManager : MonoBehaviour
    {
        public enum CameraType
        {
            FOLLOW,
            SCENE
        }

        public struct Constants
        {
            public readonly static float DEFAULT_LERP_POSITION = 0.05f;
            public readonly static float DEFAULT_LERP_ROTATION = 0.05f;
        }

        private Transform m_cam;
        private Transform m_cameraSceneSettings;
        private CameraType m_currentMode;
        private bool m_aimAtCenter;

        protected static CameraManager m_instance;
        public static CameraManager Instance
        {
            get
            {
                return m_instance;
            }
        }

        // Use this for initialization
        void Start()
        {
            m_instance = this;
            if (Camera.main != null)
            {
                m_cam = transform;
            }
            else
            {
                throw new System.Exception("No camera found");
            }

            m_currentMode = CameraType.FOLLOW;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_currentMode == CameraType.SCENE)
            {
                m_cam.position = Vector3.Lerp(m_cam.position, m_cameraSceneSettings.position, Constants.DEFAULT_LERP_POSITION);
                if(m_aimAtCenter)
                {
                    Vector3 cameraToBarycenter = PlayerManager.Instance.GetPlayerBarycenter() - m_cam.position;
                    m_cam.forward = Vector3.Lerp(m_cam.forward, cameraToBarycenter, Constants.DEFAULT_LERP_ROTATION);
                } else
                {
                    m_cam.rotation = Quaternion.Lerp(m_cam.rotation, m_cameraSceneSettings.rotation, Constants.DEFAULT_LERP_ROTATION);
                }
            }

            if(m_currentMode == CameraType.FOLLOW)
            {

            }
        }

        public void SetSceneSettings (Transform _newCameraSceneSettings, bool _aimAtCenter)
        {
            m_cameraSceneSettings = _newCameraSceneSettings;
            m_currentMode = CameraType.SCENE;
            m_aimAtCenter = _aimAtCenter;
        }
    }
}