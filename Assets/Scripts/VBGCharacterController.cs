using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterHealth))]
    public class VBGCharacterController : MonoBehaviour, IDynamic
    {

        public struct Constants
        {

            public readonly static float CHARACTER_START_HEALTH = 100.0f;
            public readonly static int ANIMATOR_LAYER_ATTACK = 0;
            public readonly static float DEATH_REVIVE_TIME = 5.0f;
            public readonly static float DEATH_REVIVE_RADIUS = 2.0f;
        }

        [System.Serializable]
        public class GameEffectCommand
        {
            public GameObject toInstanciate;
            public float cooldown = 2.0f;
            public float timer;
            public bool child;
        }

        public enum Action
        {
            NONE,
            ATTACK,
            SPE_ATTACK,
            MOVEMENT,
            SPE_MOVEMENT,
            DEFENSE,
            SPE_DEFENSE,
            SPECIAL,
            SPE_SPECIAL
        }

        public enum Character
        {
            APOLLO,
            ARES,
            ARTEMIS,
            HEPHAESTUS
        }

        public struct Request
        {
            public Vector3 move;
            public Vector3 direction;
            public float inputNorm;
            public float directionNorm;
            public Action action;
            public void Init()
            {
                move = Vector3.zero;
                direction = Vector3.zero;
                inputNorm = 0.0f;
                directionNorm = 0.0f;
                action = Action.NONE;
            }
        }

        // Components
        private Rigidbody rb;
        private CharacterHealth health;
        private Animator animator;
        private HUDHelper hudHelper;

        // Parameters
        public float speed = 6;
        public float rotationSpeedFactor = 0.2f;
        public Transform groundChecker;
        public LayerMask Ground;
        public Character character;

        // Special attacks
        public GameEffectCommand specialAttack;
        public GameEffectCommand specialMovement;
        public GameEffectCommand specialDefense;
        public GameEffectCommand specialSpecial;
        
        public GameEffectCommand movement;
        public GameEffectCommand defense;
        public GameEffectCommand special;

        // Members
        public Vector3 lastDirection;
        public Vector3 lastMove;
        public float lastInputNorm;
        public float lastDirectionNorm;
        public Action action;
        private List<GameEffect> activeGameEffects;
        private bool weaponIsActive;
        private Vector3 bodyMovement;
        public bool isGrounded;
        private float deathTimer = 0.0f;
        public bool canMove = true;


        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            activeGameEffects = new List<GameEffect>();
            health = GetComponent<CharacterHealth>();
            animator = GetComponent<Animator>();
            hudHelper = GetComponent<HUDHelper>();
            weaponIsActive = false;
            isGrounded = true;
        }

        // Update is called once per frame
        void Update()
        {
            isGrounded = Physics.CheckSphere(groundChecker.position, groundChecker.localPosition.y + 0.1f, Ground, QueryTriggerInteraction.Ignore);
            weaponIsActive = animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking") || animator.GetCurrentAnimatorStateInfo(0).IsName("Whirlwind");

            // Apply movement
            bodyMovement = rb.velocity;
            ProcessAction();
            //
            if(canMove)
            {
                bodyMovement += lastMove * lastInputNorm * speed;
                transform.forward = Vector3.Lerp(transform.forward, lastDirection, rotationSpeedFactor);
            }

            // Apply GameEffects
            activeGameEffects.RemoveAll(item => item == null);
            for (int idx = 0; idx < activeGameEffects.Count; idx++)
            {
                GameEffect ge = activeGameEffects[idx];
                ge.ProcessOnCollision(this, rb, ref bodyMovement);
            }

            Vector2 groundMovement = bodyMovement;
            groundMovement.y = 0.0f;
            animator.SetBool("Walking", groundMovement.magnitude > 0.3f);

            // Update CC
            //cc.Move(movement * Time.deltaTime);
            animator.SetFloat("Health", health.GetHealth());

            // Cooldowns
            // TODO in list
            if (health.IsDead() && tag == GameManager.Constants.TAG_PLAYER)
            {
                float reviveSpeed = GetReviveSpeed();
                deathTimer = Mathf.Clamp(deathTimer - (reviveSpeed * Time.deltaTime), 0.0f, Constants.DEATH_REVIVE_TIME);
            }

            ProcessCooldown(specialAttack);
            ProcessCooldown(specialDefense);
            ProcessCooldown(specialMovement);
            ProcessCooldown(specialSpecial);

            ProcessCooldown(movement);
            ProcessCooldown(defense);
            ProcessCooldown(special);
            
            if(health.IsDead() && deathTimer == 0.0f)
            {
                Revive();
            }
        }

        private void ProcessCooldown(GameEffectCommand gec)
        {
            if (gec != null)
            {
                gec.timer = Mathf.Clamp(gec.timer - Time.deltaTime, 0.0f, 1000);
            }
        }

        void FixedUpdate()
        {
            rb.MovePosition(rb.position + bodyMovement * Time.fixedDeltaTime);
        }

        public void Move(Request _req)
        {
            if(health.IsDead())
            {
                return;
            }
            if (_req.direction.magnitude > 0.0f)
            {
                lastDirection = _req.direction.normalized;
            }
            if (_req.move.magnitude > 0.0f)
            {
                lastMove = _req.move.normalized;
            }
            lastInputNorm = _req.inputNorm;
            lastDirectionNorm = _req.directionNorm;

            if (_req.action != Action.NONE)
            {
                action = _req.action;
            }
        }

        private void ProcessAction()
        {
            switch(action)
            {
                case Action.ATTACK:
                    animator.SetTrigger("Attack");
                break;
                case Action.SPE_ATTACK:
                case Action.SPE_DEFENSE:
                case Action.SPE_MOVEMENT:
                case Action.SPE_SPECIAL:
                case Action.MOVEMENT:
                case Action.DEFENSE:
                case Action.SPECIAL:
                    TriggerGameEffect(action);
                    break;
            }

            action = Action.NONE;
        }

        private void TriggerGameEffect(Action action)
        {
            GameEffectCommand command;
            switch(action)
            {
                case Action.SPE_ATTACK:
                    command = specialAttack;
                    break;
                case Action.SPE_MOVEMENT:
                    command = specialMovement;
                    break;
                case Action.SPE_DEFENSE:
                    command = specialDefense;
                    break;
                case Action.SPE_SPECIAL:
                    command = specialSpecial;
                    break;
                case Action.MOVEMENT:
                    command = movement;
                    break;
                case Action.DEFENSE:
                    command = defense;
                    break;
                case Action.SPECIAL:
                    command = special;
                    break;
                default:
                    Debug.Assert(false, "No prefab provided");
                    return;
            }

            if (command.toInstanciate == null)
                return;

            if(command.timer > 0.0f)
            {
                Debug.Log("Too soon");
                return;
            }

            command.timer = command.cooldown;

            GameObject geGameObject = Instantiate(command.toInstanciate);
            GameEffect gameEffect = geGameObject.GetComponent<GameEffect>();
            if(gameEffect == null)
            {
                gameEffect = geGameObject.GetComponentsInChildren<GameEffect>()[0];
            }
            Debug.Assert(gameEffect, "Instantiated prefab has no GameEffect");

            gameEffect.SetOwner(this);

            geGameObject.transform.position = transform.position;
            geGameObject.transform.forward = transform.forward;
            if(command.child)
            {
                gameEffect.FollowTransform(transform, true, false);
            }
        }

        public void RegisterGameEffect(GameEffect ge)
        {
            activeGameEffects.Add(ge);
        }

        public void UnRegisterGameEffect(GameEffect ge)
        {
            activeGameEffects.Remove(ge);
        }

        public void Damage(float intensity)
        {
            if(health == null || health.IsDead())
            {
                return;
            }
            health.Damage(intensity);

            if(health.GetHealth() == 0.0f)
            {
                Die();
            }
        }

        private void Die()
        {
            deathTimer = Constants.DEATH_REVIVE_TIME;
        }

        private void Revive()
        {
            health.SetHealth(Constants.CHARACTER_START_HEALTH);
        }

        public void Heal(float intensity)
        {
            if (health == null || health.IsDead())
            {
                return;
            }
            health.Damage(intensity);
        }

        public bool WeaponIsActive
        {
            get
            {
                return weaponIsActive;
            }
        }

        public float GetDirectionNorm()
        {
            return lastDirectionNorm;
        }

        public float GetDeathTimer()
        {
            return deathTimer;
        }

        private int GetReviveSpeed()
        {
            int ret = -1;

            List<VBGCharacterController> players = PlayerManager.Instance.GetAllPlayersInGame();

            foreach(VBGCharacterController player in players)
            {
                if(player == this)
                {
                    continue;
                }
                float distance = (player.transform.position - transform.position).magnitude;

                if(distance < Constants.DEATH_REVIVE_RADIUS)
                {
                    ret++;
                }
            }

            if(ret == 0)
            {
                ret = -1;
            }

            return ret;
        }

        public bool IsDead()
        {
            return health.IsDead();
        }

        public void SetParalyzed(bool _paralyzed)
        {
            canMove = !_paralyzed;
        }
    }
}