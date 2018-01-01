using UnityEngine;

namespace vbg
{
    public class Collider : MonoBehaviour
    {
        void Start()
        {
            GetComponent<MeshRenderer>().enabled = GameManager.DebugConstants.SHOW_COLLIDERS_INGAME;
        }
    }
}