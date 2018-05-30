using UnityEngine;

namespace vbg
{
    public class Collider : MonoBehaviour
    {
        public enum ColliderType
        {
            DEFAULT,
            STONE,
            WOOD,
            DIRT,
            METAL
        }

        public ColliderType m_groundType = ColliderType.DEFAULT;
        public bool hideRenderInGame = true;

        public static string TypeToString(ColliderType _type)
        {
            switch(_type)
            {
                case ColliderType.WOOD:
                    return "WOOD";
                case ColliderType.DIRT:
                    return "DIRT";
                case ColliderType.METAL:
                    return "METAL";
                case ColliderType.STONE:
                case ColliderType.DEFAULT:
                default:
                    return "STONE";
            }
        }

        void Start()
        {
            if(hideRenderInGame)
                GetComponent<MeshRenderer>().enabled = GameManager.Instance.showCollidersInGame && Debug.isDebugBuild;
        }

        public ColliderType GetGroundType()
        {
            return m_groundType;
        }
    }
}