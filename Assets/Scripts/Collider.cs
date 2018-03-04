using UnityEngine;

namespace vbg
{
    public class Collider : MonoBehaviour
    {
        void Start()
        {
            GetComponent<MeshRenderer>().enabled = GameManager.Instance.showCollidersInGame && Debug.isDebugBuild;
        }
    }
}