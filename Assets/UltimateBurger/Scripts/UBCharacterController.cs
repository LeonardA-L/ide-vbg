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

            float shiftFactor = Mathf.Abs(shift);

            //Debug.Log("R " + transform.up * shiftFactor * -req.RThruster);
            //Debug.Log("L " + transform.up * shiftFactor * req.LThruster);

            rb.AddRelativeTorque(transform.up * shiftFactor * -req.RThruster, ForceMode.VelocityChange);
            rb.AddRelativeTorque(transform.up * shiftFactor * req.LThruster, ForceMode.VelocityChange);

            //Debug.Log(rb.angularVelocity);

            rb.AddForce((req.LThruster + req.RThruster) * maxThruster * TransformToWorld(thrustDirection));
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