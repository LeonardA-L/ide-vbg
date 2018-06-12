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
        private float temp = 0.0f;
        private float wait = 0.3f;

        // Use this for initialization
        void Start()
        {
            goal = transform.position;
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            temp -= Time.fixedDeltaTime;
            
            if((goal - transform.position).magnitude < 0.01)
            {
                transform.position = goal;
            } else {

                transform.position = Vector3.Lerp(transform.position, goal, 0.15f);
            }

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

        public Vector3 FindClosest(Vector3 _from)
        {
            Vector3 gridForward = (new Vector3(1, 0, 1)).normalized;
            Vector3 gridBackward = -gridForward;
            Vector3 gridSide = (new Vector3(1, 0, -1)).normalized;
            Vector3 gridBackSide = -gridSide;

            Vector3 ret = gridForward;
            float max = -10;

            if(Vector3.Dot(_from, gridForward) > max)
            {
                max = Vector3.Dot(_from, gridForward);
                ret = gridForward;
            }
            if (Vector3.Dot(_from, gridBackward) > max)
            {
                max = Vector3.Dot(_from, gridBackward);
                ret = gridBackward;
            }
            if (Vector3.Dot(_from, gridSide) > max)
            {
                max = Vector3.Dot(_from, gridSide);
                ret = gridSide;
            }
            if (Vector3.Dot(_from, gridBackSide) > max)
            {
                max = Vector3.Dot(_from, gridBackSide);
                ret = gridBackSide;
            }


            return ret;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (lockPos)
                return;
            if (temp > 0)
                return;
            if(collision.transform.tag == GameManager.Constants.TAG_PLAYER)
            {

                Vector3 disp = transform.position - collision.transform.position;
                disp.y = 0;
                /*if (Mathf.Abs(disp.x) > Mathf.Abs(disp.z))
                {
                    disp.z = 0;
                }
                else
                {
                    disp.x = 0;
                }*/

                disp = FindClosest(disp);

                disp.Normalize();

                RaycastHit[] hits;
                hits = Physics.RaycastAll(transform.position, disp, 2.0f);
                Debug.DrawLine(transform.position, transform.position + disp * 2, Color.blue, 2.0f);

                int i = hits.Length;
                foreach(var h in hits)
                {
                    if(h.collider.tag == GameManager.Constants.TAG_NONTRIGGERCOLLIDER || h.collider.gameObject.layer == GameManager.Constants.LAYER_SOUND)
                    {
                        i--;
                    }
                }

                if (i == 0)
                {
                    goal += disp * 2;
                    temp = wait;
                    SoundManager.Instance.PostEvent("Play_PUZZLEBOX", gameObject);
                }
            }
        }

        public void Lock()
        {
            lockPos = true;
        }

        public void Reset(Vector3 newPosition)
        {
            goal = newPosition;
            rb.velocity = new Vector3();
        }
    }
}