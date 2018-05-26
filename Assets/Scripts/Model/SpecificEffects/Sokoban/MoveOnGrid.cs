using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class MoveOnGrid : MonoBehaviour
    {
        private Vector3 goal;
        public bool lockPos = false;
        public Rigidbody rb;

        // Use this for initialization
        void Start()
        {
            goal = transform.position;
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, goal, 0.15f);
            //rb.MovePosition(Vector3.Lerp(transform.position, goal, 0.3f));
            /*if (lastPosition != transform.position)
            {
                Vector3 disp = transform.position - lastPosition;
                if (Mathf.Abs(disp.x) > Mathf.Abs(disp.z))
                {
                    disp.z = 0;
                }
                else
                {
                    disp.x = 0;
                }
                transform.position = lastPosition + disp;
            }*/

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (lockPos)
                return;
            if(collision.transform.tag == GameManager.Constants.TAG_PLAYER)
            {
                Vector3 disp = transform.position - collision.transform.position;
                disp.y = 0;
                if (Mathf.Abs(disp.x) > Mathf.Abs(disp.z))
                {
                    disp.z = 0;
                }
                else
                {
                    disp.x = 0;
                }

                disp.Normalize();

                RaycastHit[] hits;
                hits = Physics.RaycastAll(transform.position, disp, 2.0f);
                Debug.DrawLine(transform.position, transform.position + disp * 2, Color.blue, 2.0f);
                Debug.Log(hits.Length);

                if(hits.Length == 0)
                    goal += disp * 2;
            }
        }

        public void Lock()
        {
            lockPos = true;
        }

        public void Reset()
        {
            goal = transform.position;
        }
    }
}