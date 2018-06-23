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

            public bool RBrake;
            public bool LBrake;
        }

        private UBUserInput inputs;
        private Rigidbody rb;
        public float maxThruster = 50;
        public float maxBrake = 10;
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

            float shiftFactor = Mathf.Abs(shift);

            //Debug.Log("R " + transform.up * shiftFactor * -req.RThruster);
            //Debug.Log("L " + transform.up * shiftFactor * req.LThruster);

            float rBrakeEffect = maxBrake * (req.RBrake ? 1.0f : 0);
            float lBrakeEffect = maxBrake * (req.LBrake ? 1.0f : 0);

            rb.AddRelativeTorque(transform.up * shiftFactor * -(req.RThruster - rBrakeEffect), ForceMode.VelocityChange);
            rb.AddRelativeTorque(transform.up * shiftFactor * (req.LThruster - lBrakeEffect), ForceMode.VelocityChange);

            //Debug.Log(rb.angularVelocity);

            rb.AddForce((req.LThruster + req.RThruster - rBrakeEffect - lBrakeEffect) * maxThruster * TransformToWorld(thrustDirection));
            /*
            RaycastHit hit;
            Ray groundRay = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(groundRay, out hit, groundCheckDist + 0.15f, Ground, QueryTriggerInteraction.Ignore))
            {
                Transform ground = hit.collider.gameObject.transform;
                Debug.Log(ground);
                transform.up = ground.up;
            }
            */
        }

        Vector3 TransformToWorld(Vector3 from)
        {
            return from.x * transform.right
                + from.y * transform.up
                + from.z * transform.forward;
        }
        /*
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.layer == Ground)
            {
                Transform ground = collision.collider.transform;
                Debug.Log(ground);
                transform.up = ground.up;
            }
        }
        */
    }
}