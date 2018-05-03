using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace vbg
{
    public class CharacterAIControl : MonoBehaviour
    {
        public Transform m_target;
        private Transform m_bestTarget;
        private VBGCharacterController m_character; // A reference to the ThirdPersonCharacter on the object
        private CharacterHealth m_health; // A reference to the ThirdPersonCharacter on the object
        private NavMeshPath m_path;
        private Animator m_aiAnimator;
        private IDynamic m_targetDynamic;

        private ChildCollider m_earRange;

        public VBGAIState m_currentState;
        public float m_elapsedInCurrentState;
        public List<Transform> m_patrolHotspots = new List<Transform>();
        public int m_currentPatrolTargetPoint = 0;
        public bool m_alwaysResetTarget = true;
        public float m_attackRange = 1.0f;

        public enum VBGAIState
        {
            IDLE,
            PICK_TARGET,
            REACH_TARGET,
            ATTACK,
            DIE,
            SPECIAL,

            IDLE_GET_POINT,
            IDLE_REACH_POINT,
        }

        // Use this for initialization
        void Start()
        {
            m_path = new NavMeshPath();
            m_character = GetComponent<VBGCharacterController>();
            m_aiAnimator = transform.Find("AI").GetComponent<Animator>();
            Transform earRangeGO = transform.Find("EarRange");
            m_earRange = earRangeGO ? earRangeGO.GetComponent<ChildCollider>() : null;
            m_health = GetComponent<CharacterHealth>();
        }

        // Update is called once per frame
        void Update()
        {
            VBGCharacterController.Request request = new VBGCharacterController.Request();
            request.Init();

            m_aiAnimator.SetFloat("Health", m_health.GetHealth());
            m_aiAnimator.SetBool("IsDead", m_health.IsDead());

            ComputeBestTarget();
            if(m_bestTarget == null && m_alwaysResetTarget)
            {
                m_target = null;
                m_targetDynamic = null;
            }

            m_aiAnimator.SetBool("BetterTargetAvailable", m_target != m_bestTarget);

            if (m_target != null)
            {
                if(m_targetDynamic == null)
                {
                    m_targetDynamic = m_target.GetComponent<IDynamic>();
                }
                Vector3 distance = m_target.transform.position - transform.position;

                Debug.DrawLine(m_target.transform.position, m_target.transform.position + (m_attackRange + m_targetDynamic.GetRadius()) * distance.normalized, Color.red, 10.0f);

                m_aiAnimator.SetBool("TargetInAttackReach", distance.magnitude < (m_attackRange + m_targetDynamic.GetRadius()));
                m_aiAnimator.SetBool("NoTarget", false);
                m_aiAnimator.SetFloat("TargetDistance", distance.magnitude);
            }
            else
            {
                m_aiAnimator.SetBool("TargetInAttackReach", false);
                m_aiAnimator.SetBool("NoTarget", true);
            }

            FSMFrame(ref request);

            m_aiAnimator.SetFloat("EslapsedTimeInState", m_elapsedInCurrentState);

            m_character.Move(request);

        }

        void ComputeBestTarget()
        {
            m_bestTarget = null;
            float minScore = float.MaxValue;

            List<VBGCharacterController> potentialTargets = PlayerManager.Instance.GetAllPlayersInGame();
            if(m_earRange != null)
            {
                potentialTargets = m_earRange.GetCharactersInRange();
            }

            foreach(VBGCharacterController ch in potentialTargets)
            {
                if(ch.tag != this.tag)
                {
                    if (!ch.IsDead() && (m_bestTarget == null || TargetScore(ch) < minScore))
                    {
                        m_bestTarget = ch.transform;
                    }
                }
            }
        }

        float TargetScore(VBGCharacterController cc)
        {
            return (cc.transform.position - transform.position).magnitude;
        }

        void FSMFrame(ref VBGCharacterController.Request _request)
        {
            VBGAIState lastState = m_currentState;
            m_currentState = GetCurrentState();

            if(m_currentState != lastState)
            {
                m_elapsedInCurrentState = 0.0f;
            } else
            {
                m_elapsedInCurrentState += Time.deltaTime;
            }

            switch(m_currentState)
            {
                case VBGAIState.ATTACK:
                    StateAttack(ref _request);
                break;
                case VBGAIState.REACH_TARGET:
                    StateReachTarget(ref _request);
                break;
                case VBGAIState.PICK_TARGET:
                    StatePickTarget(ref _request);
                    break;
                case VBGAIState.SPECIAL:
                    StateSpecial(ref _request);
                    break;
                case VBGAIState.IDLE_GET_POINT:
                    StateIdleGetPoint(ref _request);
                    break;
                case VBGAIState.IDLE_REACH_POINT:
                    StateIdleReachPoint(ref _request);
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
            if (state.IsName("PickTarget"))
                return VBGAIState.PICK_TARGET;
            if (state.IsName("Attack"))
                return VBGAIState.ATTACK;
            if (state.IsName("Die"))
                return VBGAIState.DIE;
            if (state.IsName("Special"))
                return VBGAIState.SPECIAL;
            if (state.IsName("Idle.GetPoint"))
                return VBGAIState.IDLE_GET_POINT;
            if (state.IsName("Idle.ReachPoint"))
                return VBGAIState.IDLE_REACH_POINT;

            Debug.Assert(false, "Getting current state failed");
            return VBGAIState.IDLE;
        }

        void GoTo(Transform t, ref VBGCharacterController.Request _request)
        {
            NavMesh.CalculatePath(transform.position, t.position, NavMesh.AllAreas, m_path);

            bool wantsToMove = false;

            if (m_path.status == NavMeshPathStatus.PathComplete)
            {
                wantsToMove = true;
                Vector3 direction = transform.forward;
                if (m_path.corners.Length > 1)
                {
                    direction = m_path.corners[1] - m_path.corners[0];
                } else
                {
                    direction = t.position - transform.position;
                }
                direction.y = 0.0f;
                //transform.forward = Vector3.Lerp(transform.forward, direction, 1.0f);

                VBGCharacterController.Action action = VBGCharacterController.Action.NONE;

                Debug.DrawLine(transform.position, transform.position + direction);

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
            m_target = null;
            m_target = null;
        }

        void StateSpecial(ref VBGCharacterController.Request _request)
        {
            TurnTo(m_target, ref _request);
            _request.action = VBGCharacterController.Action.SPECIAL;
        }

        void StatePickTarget(ref VBGCharacterController.Request _request)
        {
            ComputeBestTarget();
            if(m_bestTarget != null)
            {
                Debug.Log("Best " + m_bestTarget);
                m_target = m_bestTarget;
                m_target = null;
                m_aiAnimator.SetTrigger("Success");
            } else
            {
                m_aiAnimator.SetTrigger("Failure");
            }
        }

        void StateIdleGetPoint(ref VBGCharacterController.Request _request)
        {
            if (m_patrolHotspots.Count == 0 || m_patrolHotspots[m_currentPatrolTargetPoint] == null)
            {
                m_aiAnimator.SetTrigger("Failure");
            } else
            {
                m_aiAnimator.SetTrigger("Success");
            }
        }

        void StateIdleReachPoint(ref VBGCharacterController.Request _request)
        {
            Transform patrolHotspot = m_patrolHotspots[m_currentPatrolTargetPoint];
            GoTo(patrolHotspot, ref _request);
            
            if((transform.position - patrolHotspot.position).magnitude < 1.0f)
            {
                m_currentPatrolTargetPoint = (m_currentPatrolTargetPoint + 1) % m_patrolHotspots.Count;
                m_aiAnimator.SetTrigger("Success");
            }
        }
    }
}