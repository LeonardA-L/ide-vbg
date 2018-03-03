using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace vbg
{
    public class CharacterAIControl : MonoBehaviour
    {
        public Transform m_target;
        private VBGCharacterController m_character; // A reference to the ThirdPersonCharacter on the object
        private CharacterHealth m_health; // A reference to the ThirdPersonCharacter on the object
        private NavMeshPath m_path;
        private Animator m_aiAnimator;

        enum VBGAIState
        {
            IDLE,
            REACH_TARGET,
            ATTACK
        }

        // Use this for initialization
        void Start()
        {
            m_path = new NavMeshPath();
            m_character = GetComponent<VBGCharacterController>();
            m_aiAnimator = transform.Find("AI").GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            VBGCharacterController.Request request = new VBGCharacterController.Request();
            request.Init();

            m_aiAnimator.SetFloat("Health", m_health.GetHealth());

            if (m_target != null)
            {
                m_aiAnimator.SetTrigger("HasTarget");

                Vector3 distance = m_target.transform.position - transform.position;

                m_aiAnimator.SetBool("TargetInAttackReach", distance.magnitude < 2.5f);
            }

            FSMFrame(ref request);

            m_character.Move(request);

        }

        void FSMFrame(ref VBGCharacterController.Request _request)
        {
            VBGAIState currentState = GetCurrentState();

            switch(currentState)
            {
                case VBGAIState.ATTACK:
                    StateAttack(ref _request);
                break;
                case VBGAIState.REACH_TARGET:
                    StateReachTarget(ref _request);
                break;
                case VBGAIState.IDLE:
                default:
                    StateIdle(ref _request);
                break;
            }
        }

        VBGAIState GetCurrentState()
        {
            AnimatorStateInfo state = m_aiAnimator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("Idle"))
                return VBGAIState.IDLE;
            if (state.IsName("ReachTarget"))
                return VBGAIState.REACH_TARGET;
            if (state.IsName("Attack"))
                return VBGAIState.ATTACK;

            Debug.Assert(false, "Getting current state failed");
            return VBGAIState.IDLE;
        }

        void GoTo(Transform t, ref VBGCharacterController.Request _request)
        {
            NavMesh.CalculatePath(transform.position, t.position, NavMesh.AllAreas, m_path);
            //Debug.Log(m_path.status);

            bool wantsToMove = false;

            if (m_path.status == NavMeshPathStatus.PathComplete)
            {
                wantsToMove = true;
                Vector3 direction = transform.forward;
                if (m_path.corners.Length > 1)
                {
                    direction = m_path.corners[1] - m_path.corners[0];
                }
                direction.y = 0.0f;
                //transform.forward = Vector3.Lerp(transform.forward, direction, 1.0f);

                VBGCharacterController.Action action = VBGCharacterController.Action.NONE;

                _request.move = direction;
                _request.direction = direction;
                _request.inputNorm = wantsToMove ? 1.0f : 0.0f;
                _request.action = action;
                _request.directionNorm = 1.0f;

            }
        }

        void TurnTo(Transform t, ref VBGCharacterController.Request _request)
        {
            Vector3 direction = m_target.transform.position - transform.position;
            _request.direction = direction;
            _request.directionNorm = 1.0f;

        }

        // State functions

        void StateAttack(ref VBGCharacterController.Request _request)
        {
            TurnTo(m_target, ref _request);
            _request.action = VBGCharacterController.Action.ATTACK;
        }

        void StateReachTarget(ref VBGCharacterController.Request _request)
        {
            GoTo(m_target, ref _request);
        }

        void StateIdle(ref VBGCharacterController.Request _request)
        {
        }
    }
}