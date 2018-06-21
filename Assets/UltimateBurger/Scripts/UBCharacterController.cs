using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vbg;

namespace ub
{
    public class UBCharacterController : MonoBehaviour
    {
        public struct Request
        {
            public float LThruster;
            public float RThruster;
        }

        private UBUserInput inputs;
        private Rigidbody rb;
        public float maxThruster = 6;
        public float shift = 0.5f;
        public float speedToShift= 0.01f;

        private Vector3 thrustDirection = new Vector3(0, 0, 1);

        // Use this for initialization
        void Start()
        {
            inputs = GetComponent<UBUserInput>();
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            Request req = inputs.GetRequest();

            Debug.Log(req.LThruster + " - " + req.RThruster);

            float shiftFactor = Mathf.Abs(shift);

            rb.AddTorque(transform.up * shiftFactor * -req.LThruster, ForceMode.Acceleration);
            rb.AddTorque(transform.up * shiftFactor * req.RThruster, ForceMode.Acceleration);

            rb.AddForce((req.LThruster + req.RThruster) * maxThruster * TransformToWorld(thrustDirection));
        }

        Vector3 TransformToWorld(Vector3 from)
        {
            return from.x * transform.right
                + from.y * transform.up
                + from.z * transform.forward;
        }
    }
}