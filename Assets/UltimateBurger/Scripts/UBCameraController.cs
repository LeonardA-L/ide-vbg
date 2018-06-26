using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vbg;

namespace ub
{
    public class UBCameraController : MonoBehaviour
    {
        [Range(0, 1)]
        public float lerp = 0.3f;
        [Range(0, 1)]
        public float lerpR = 0.3f;
        [Range(0, 1)]
        public float lerpF = 0.3f;
        [Range(0, 1)]
        public float lerpRoll = 0.3f;
        public float angleS = 22.0f;
        public float angleE = 15.0f;
        public float distanceS = -3.5f;
        public float distanceE = -2;
        public float fovS = 45;
        public float fovE = 60;
        public float rollE = 8.0f;
        public float speedMax = 60;
        public float angSpeedMax = 2.0f;
        private float rollGoal = 0.0f;
        public GameObject target;
        public Camera camera;
        private Transform targetTransform;
        private UBCharacterController targetCC;
        private Rigidbody targerRb;
        // Use this for initialization
        void Start()
        {
            targetTransform = target.transform;
            targetCC = target.GetComponent<UBCharacterController>();
            targerRb = target.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            if (targerRb == null)
                return;
            transform.position -= transform.up * 2.0f;  // Cheap offset hack

            float velocityRatio = Mathf.Clamp(targerRb.velocity.magnitude / speedMax, 0.0f, 1.0f);
            float smoothedRatio = Easings.Interpolate(velocityRatio, Easings.Functions.QuadraticEaseInOut);
            float angularVelocityRatio = Mathf.Clamp(targerRb.angularVelocity.y / angSpeedMax, -1.0f, 1.0f);

            Vector3 dir = targetTransform.forward;
            dir = Quaternion.AngleAxis(((angleE - angleS) * smoothedRatio + angleS), targetTransform.right) * dir;
            Vector3 goal = targetTransform.position - ((distanceE - distanceS) * smoothedRatio + distanceS) * dir;

            transform.position = Vector3.Slerp(transform.position, goal, lerp * 60 * Time.deltaTime);
            transform.forward = Vector3.Slerp(transform.forward, dir, lerpR * 60 * Time.deltaTime);

            float fovGoal = fovS;
            Vector3 planeForward = transform.forward;
            planeForward.y = 0.0f;
            planeForward.Normalize();
            if (Vector3.Dot(targerRb.velocity, planeForward) >= 0.0f)
            {
                fovGoal = (fovE - fovS) * smoothedRatio + 45;
            }
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, fovGoal, lerpF * 60 * Time.deltaTime);


            rollGoal = Mathf.Lerp(rollGoal, rollE * angularVelocityRatio, lerpRoll * 60 * Time.deltaTime);
            //camera.transform.Rotate(camera.transform.forward, rollGoal);
            //camera.transform.eulerAngles = new Vector3(rollGoal, 0, 0);
            camera.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rollGoal));
            transform.position += transform.up * 2.0f; // Cheap offset hack
        }
    }
}