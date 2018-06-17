using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [ExecuteInEditMode]
    public class DynamicCount : MonoBehaviour
    {
#if UNITY_EDITOR
        // Use this for initialization
        void Awake ()
        {
            var dynamics = FindObjectsOfType<Dynamic>();
            Debug.Log("Dynamic : " + dynamics.Length);
            var meshCo = FindObjectsOfType<MeshCollider>();
            Debug.Log("MeshCollider : " + meshCo.Length);
        }
#endif
    }
}