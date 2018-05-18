using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class VFXSpawner : MonoBehaviour
    {
        public GameObject vfxPrefab;
        public bool followParentPosition = false;
        public bool followParentRotation = false;
        public bool endOnDestroy = true;

        private VFX vfx;

        // Use this for initialization
        void Start()
        {
            GameObject go = GameObject.Instantiate(vfxPrefab);
            Transform tr = go.transform;

            vfx = go.GetComponent<VFX>();

            tr.position = transform.parent.position + transform.localPosition;
            tr.rotation = transform.parent.rotation;

            vfx.SetFXData(followParentRotation, followParentPosition ? transform.parent : null);
        }

        private void OnDestroy()
        {
            if(endOnDestroy && vfx != null)
            {
                vfx.End();
            }
        }
    }
}