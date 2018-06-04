using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SkeletonKOTHController : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            CharacterAIControl AI = GetComponent<CharacterAIControl>();
            Transform coreTransform = GameObject.Find("Core").transform;
            AI.SetTarget(coreTransform);
        }
    }
}