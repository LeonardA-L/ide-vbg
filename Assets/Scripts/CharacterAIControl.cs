using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace vbg
{
    public class CharacterAIControl : MonoBehaviour
    {
        public Transform target;
        private VBGCharacterController m_Character; // A reference to the ThirdPersonCharacter on the object
        private NavMeshPath m_path;

        // Use this for initialization
        void Start()
        {
            m_path = new NavMeshPath();
            m_Character = GetComponent<VBGCharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            if(target != null)
            {
                NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, m_path);
                Debug.Log(m_path.status);

                bool wantsToMove = false;

                if (m_path.status == NavMeshPathStatus.PathComplete) {
                    wantsToMove = true;
                    Vector3 direction = transform.forward;
                    if (m_path.corners.Length > 1)
                    {
                        direction = m_path.corners[1] - m_path.corners[0];
                    }
                    direction.y = 0.0f;
                    //transform.forward = Vector3.Lerp(transform.forward, direction, 1.0f);

                    VBGCharacterController.Action action = VBGCharacterController.Action.NONE;

                    VBGCharacterController.Request request = new VBGCharacterController.Request
                    {
                        move = direction,
                        direction = direction,
                        inputNorm = wantsToMove ? 1.0f : 0.0f,
                        action = action,
                        directionNorm = 1.0f
                    };

                    // pass all parameters to the character control script
                    m_Character.Move(request);
                }
                //Debug.Log(path.corners.Length);

                //
                
            }
        }
    }
}