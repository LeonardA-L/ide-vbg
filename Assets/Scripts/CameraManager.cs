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
            SCENE,
            ANIMATED
        }

        public struct Constants
        {
            public readonly static float DEFAULT_LERP_POSITION = 0.05f;
            public readonly static float DEFAULT_LERP_ROTATION = 0.05f;
        }

        private Transform m_cam;
        private Transform m_cameraToFollow;
        private CameraType m_currentMode;
        private bool m_aimAtCenter;
        private float m_positionSmooth;
        private float m_rotationSmooth;

        private Animator m_refAnimator;
        private Transform m_hotspotStart;
        private Transform m_hotspotEnd;
        private float m_actualRatio;

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

            m_actualRatio = -1;

            m_currentMode = CameraType.FOLLOW;
            m_positionSmooth = Constants.DEFAULT_LERP_POSITION;
            m_rotationSmooth = Constants.DEFAULT_LERP_ROTATION;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_currentMode == CameraType.SCENE)
            {
                m_cam.position = Vector3.Lerp(m_cam.position, m_cameraToFollow.position, m_positionSmooth);
                if(m_aimAtCenter && PlayerManager.Instance.GetPlayersInGame() > 0)
                {
                    Vector3 cameraToBarycenter = PlayerManager.Instance.GetPlayerBarycenter() - m_cam.position;
                    m_cam.forward = Vector3.Lerp(m_cam.forward, cameraToBarycenter, m_rotationSmooth);
                } else
                {
                    m_cam.rotation = Quaternion.Lerp(m_cam.rotation, m_cameraToFollow.rotation, m_rotationSmooth);
                }
            }

            if (m_currentMode == CameraType.ANIMATED && PlayerManager.Instance.GetPlayersInGame() > 0)
            {
                // Compute position ratio
                Vector3 centerPosition = PlayerManager.Instance.GetPlayerBarycenter();
                Vector3 hotspotsSegment = (m_hotspotEnd.position - m_hotspotStart.position);
                Vector3 hotspotToCenter = (centerPosition - m_hotspotStart.position);

                float dot = Vector3.Dot(hotspotToCenter, hotspotsSegment) / hotspotsSegment.magnitude / hotspotsSegment.magnitude;
                //Debug.Log(dot);
                dot = Mathf.Clamp(dot, 0.0f, 0.999f);

                m_actualRatio = Mathf.Lerp(m_actualRatio, dot, m_actualRatio >= 0.0f ? 0.1f : 1.0f);

                // Set Animator ratio
                m_refAnimator.SetFloat("Ratio", m_actualRatio);

                // Apply changes
                m_cam.position = Vector3.Lerp(m_cam.position, m_cameraToFollow.position, m_positionSmooth);
                m_cam.rotation = Quaternion.Lerp(m_cam.rotation, m_cameraToFollow.rotation, m_rotationSmooth);
            }

            if (m_currentMode == CameraType.FOLLOW)
            {

            }
        }

        public void SetSceneSettings (Transform _newCameraToFollow, bool _aimAtCenter, float _overridePositionSmooth = 0.0f, float _overrideRotationSmooth = 0.0f)
        {
            m_cameraToFollow = _newCameraToFollow;
            m_currentMode = CameraType.SCENE;
            m_aimAtCenter = _aimAtCenter;

            m_positionSmooth = _overridePositionSmooth;
            m_rotationSmooth = _overrideRotationSmooth;
        }

        public void SetAnimatedSettings(Transform _newCameraToFollow, Transform _hotspotStart, Transform _hotspotEnd, Animator _cameraAnimator)
        {
            m_currentMode = CameraType.ANIMATED;
            m_hotspotStart = _hotspotStart;
            m_hotspotEnd = _hotspotEnd;
            m_cameraToFollow = _newCameraToFollow;
            m_refAnimator = _cameraAnimator;
            m_actualRatio = -1;
        }
    }
}