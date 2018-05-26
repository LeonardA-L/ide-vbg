using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class LockOnCollision : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            MoveOnGrid mov = collision.transform.GetComponent<MoveOnGrid>();
            if(mov != null)
            {
                mov.Lock();
            }
        }
    }
}