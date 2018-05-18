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
            ANIMATED,
            NONE
        }

        public struct Constants
        {
            public readonly static float DEFAULT_LERP_POSITION = 0.05f;
            public readonly static float DEFAULT_LERP_ROTATION = 0.01f;
            public readonly static float DEFAULT_FOLLOW_DISTANCE = 20.0f;
            public readonly static int CACHED_BARYCENTERS = 10;
        }

        private Transform m_cam;
        private Transform m_cameraToFollow;
        private CameraType m_currentMode;
        private bool m_aimAtCenter;
        private float m_positionSmooth;
        private float m_rotationSmooth;
        private float m_enlargeD1 = 3;
        private float m_enlargeD2 = 10;
        private float m_enlargeF1 = 25;
        private float m_enlargeF2 = 45;

        private Animator m_refAnimator;
        private Transform m_hotspotStart;
        private Transform m_hotspotEnd;
        private float m_actualRatio;

        private List<Vector3> m_cachedBarycenters = new List<Vector3>();
        public float m_predictionFactor = 1;
        public float m_speedPredictionFactor = 1.0f;

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

            m_currentMode = CameraType.NONE;
            m_positionSmooth = Constants.DEFAULT_LERP_POSITION;
            m_rotationSmooth = Constants.DEFAULT_LERP_ROTATION;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_currentMode == CameraType.SCENE)
            {
                m_cam.position = Vector3.Lerp(m_cam.position, m_cameraToFollow.position, m_positionSmooth);
                if(m_aimAtCenter && PlayerManager.Instance.GetPlayersInGameAmount() > 0)
                {
                    Vector3 cameraToBarycenter = PlayerManager.Instance.GetPlayerBarycenter() - m_cam.position;
                    m_cam.forward = Vector3.Lerp(m_cam.forward, cameraToBarycenter, m_rotationSmooth);
                } else
                {
                    m_cam.rotation = Quaternion.Lerp(m_cam.rotation, m_cameraToFollow.rotation, m_rotationSmooth);
                }
            }

            if (m_currentMode == CameraType.ANIMATED && PlayerManager.Instance.GetPlayersInGameAmount() > 0)
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
                if (PlayerManager.Instance.GetPlayersInGameAmount() > 0)
                {
                    Vector3 barycenter = PlayerManager.Instance.GetPlayerBarycenter();

                    if(m_cachedBarycenters.Count == 0 || m_cachedBarycenters[m_cachedBarycenters.Count - 1] != barycenter)
                        m_cachedBarycenters.Add(barycenter);
                    if (m_cachedBarycenters.Count > Constants.CACHED_BARYCENTERS)
                        m_cachedBarycenters.RemoveAt(0);
                    float barycenterSpeedFactor = 0;

                    Vector3 predictedPosition = new Vector3();
                    for(int i=1;i<m_cachedBarycenters.Count; i++)
                    {
                        Debug.DrawLine(m_cachedBarycenters[i], m_cachedBarycenters[i] + new Vector3(0, 3, 0), Color.blue);
                        predictedPosition += (m_cachedBarycenters[i] - m_cachedBarycenters[i-1]).normalized;
                        barycenterSpeedFactor += (m_cachedBarycenters[i] - m_cachedBarycenters[i - 1]).magnitude;
                    }
                    predictedPosition.Normalize();
                    predictedPosition *= (m_predictionFactor + barycenterSpeedFactor * m_speedPredictionFactor);
                    Debug.DrawLine(barycenter, barycenter + predictedPosition, Color.red, 2.0f);
                    barycenter += predictedPosition;


                    float playerRadius = PlayerManager.Instance.GetPlayersRadius();
                    //Debug.Log(playerRadius);
                    Vector3 cameraToBarycenter = predictedPosition - m_cam.position;
                    //m_cam.forward = Vector3.Lerp(m_cam.forward, cameraToBarycenter, m_rotationSmooth);

                    if(m_cameraToFollow != null)
                        m_cam.forward = Vector3.Lerp(m_cam.forward, m_cameraToFollow.forward, m_rotationSmooth);

                    float followDistance = 25.0f;

                    if(playerRadius < m_enlargeD1)
                    {
                        followDistance = m_enlargeF1;
                    }
                    else if (playerRadius > m_enlargeD2)
                    {
                        followDistance = m_enlargeF2;
                    } else
                    {
                        float a = (m_enlargeF2 - m_enlargeF1) / (m_enlargeD2 - m_enlargeD1);
                        float b = m_enlargeF1 - m_enlargeD1 * a;
                        followDistance = a * playerRadius + b;
                    }

                    //Debug.Log(playerRadius + " - " + followDistance);

                    m_cam.transform.position = Vector3.Lerp(m_cam.transform.position, barycenter - m_cam.forward * followDistance, Constants.DEFAULT_LERP_POSITION);
                }
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

        public void SetFollowSettings(Transform _newCameraToFollow, float _d1, float _d2, float _f1, float _f2, float _overridePositionSmooth = 0.0f, float _overrideRotationSmooth = 0.0f)
        {
            m_cameraToFollow = _newCameraToFollow;
            m_currentMode = CameraType.FOLLOW;

            m_enlargeD1 = _d1;
            m_enlargeD2 = _d2;
            m_enlargeF1 = _f1;
            m_enlargeF2 = _f2;

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