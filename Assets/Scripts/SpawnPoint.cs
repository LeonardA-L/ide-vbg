using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class SpawnPoint : MonoBehaviour
    {
        public bool start = false;
        public bool spawnpoint = true;

        public bool IsStartPoint()
        {
            return start;
        }
    }
}