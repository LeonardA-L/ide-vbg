using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof(Animator))]
    public class Destroyable : MonoBehaviour
    {

        public void DestroyMe()
        {
            Destroy(gameObject);
        }
    }

}