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
            DIRT
        }

        public ColliderType m_groundType = ColliderType.DEFAULT;
        public bool hideRenderInGame = true;

        public static string TypeToString(ColliderType _type)
        {
            switch(_type)
            {
                case ColliderType.STONE:
                    return "STONE";
                case ColliderType.WOOD:
                    return "WOOD";
                case ColliderType.DIRT:
                    return "DIRT";
                case ColliderType.DEFAULT:
                default:
                    return "DEFAULT";
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