using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SokobanReset : MonoBehaviour
    {
        public Transform crate1;
        public Transform crate2;
        private Vector3 crate1StartPos;
        private Vector3 crate2StartPos;

        public Transform target1;
        public Transform target2;

        private float timer = 1;

        // Use this for initialization
        void Start()
        {
            crate1StartPos = crate1.position;
            crate2StartPos = crate2.position;

        }

        // Update is called once per frame
        void Update()
        {
            if (!SwitchManager.Instance.GetSwitch("SokoStart"))
                return;

            timer -= Time.deltaTime;
            timer = Mathf.Max(0, timer);
            if (timer > 0)
                return;

            int k = 0;
            Vector3 t1d = target1.position - crate1.position;
            t1d.y = 0;
            if(t1d.magnitude < 1.0f)
            {
                k++;
            }

            Vector3 t2d = target2.position - crate2.position;
            t2d.y = 0;
            if (t2d.magnitude < 1.0f)
            {
                k++;
            }

            if(k == 2)
            {
                SwitchManager.Instance.SetSwitch("Sokoban", true);
            }
        }

        public void Reset()
        {
            if (SwitchManager.Instance.GetSwitch("Sokoban"))
                return;

            crate1.GetComponent<MoveOnGrid>().Reset(crate1StartPos);
            crate2.GetComponent<MoveOnGrid>().Reset(crate2StartPos);

        }
    }
}
