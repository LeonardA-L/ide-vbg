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

            public readonly static int COMBO_STRIKE_FRAME_INF = 2;
            public readonly static int COMBO_STRIKE_FRAME_SUP = 40;
            public readonly static int COMBO_STRIKE_MAX = 3;
        }

        [System.Serializable]
        public class GameEffectCommand
        {
            public GameObject toInstanciate;
            public float cooldown = 2.0f;
            public float timer;
            public bool child;
            public bool unique;
            public GameObject previous;
        }

        public enum AnimatorLayer
        {
            DEFAULT,
            UPPER
        }

        public enum Action
        {
            NONE,
            ATTACK,
            ATTACK_AIM,
            SPE_ATTACK,
            SPE_ATTACK_AIM,
            MOVEMENT,
            SPE_MOVEMENT,
            DEFENSE,
            SPE_DEFENSE,
            SPE_DEFENSE_AIM,
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
            public bool modifier;
            public bool strafe;
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
        private UnityEngine.Collider col;
        private CharacterHealth health;
        private Animator animator;
        private HUDHelper hudHelper;

        // Parameters
        public float speed = 6;
        public float rotationSpeedFactor = 0.2f;
        public Transform groundChecker;
        public LayerMask Ground;
        public Character character;
        public bool destroyOnDie = false;

        // Special attacks
        public GameEffectCommand specialAttack;
        public GameEffectCommand specialMovement;
        public GameEffectCommand specialDefense;
        public GameEffectCommand specialSpecial;
        
        public GameEffectCommand movement;
        public GameEffectCommand defense;
        public GameEffectCommand special;

        public List<GameEffectCommand> additionnalCommands = new List<GameEffectCommand>();

        // Members
        public Vector3 lastDirection;
        public Vector3 lastMove;
        public float lastInputNorm;
        public float lastDirectionNorm;
        public bool lastStrafe = false;
        public Action action;
        private List<GameEffect> activeGameEffects;
        private bool weaponIsActive;
        private Vector3 bodyMovement;
        public bool isGrounded;
        private float deathTimer = 0.0f;
        public bool canMove = true;
        private bool superMode = false;
        public List<Animator> additionalAnimators = new List<Animator>();
        public float speedFactor = 1.0f;
        public float rotFactor = 1.0f;
        public bool blockActions = false;
        private float radius = 1.0f;
        private bool isPlayer;
        private long frame = 0;
        private long comboAttackLastFrame = 0;
        private int comboAttackLevel = 0;

        [Tooltip("A prefab to instantiate when the character dies")]
        public GameObject finishPrefab;

        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            activeGameEffects = new List<GameEffect>();
            health = GetComponent<CharacterHealth>();
            animator = GetComponent<Animator>();
            isPlayer = GetComponent<CharacterUserControl>() != null;
            Transform hudHelperObject = transform.Find("HUD");
            if (hudHelperObject != null)
            {
                hudHelper = hudHelperObject.gameObject.GetComponent<HUDHelper>();
            }
            col = GetComponent<UnityEngine.Collider>();
            weaponIsActive = false;
            isGrounded = true;
        }

        // Update is called once per frame
        void Update()
        {
            frame++;
            float stableTimeRatio = Time.deltaTime * GameManager.Constants.FPS_REF;
            AnimatorSetFloat("RND", Random.Range(0, 100));

            isGrounded = Physics.CheckSphere(groundChecker.position, groundChecker.localPosition.y + 0.15f, Ground, QueryTriggerInteraction.Ignore);
            AnimatorSetBool("Grounded", isGrounded);

            //weaponIsActive = animator.GetCurrentAnimatorStateInfo((int)AnimatorLayer.UPPER).IsName("Attacking") || animator.GetCurrentAnimatorStateInfo((int)AnimatorLayer.DEFAULT).IsName("Whirlwind");

            // Apply movement
            bodyMovement = rb.velocity;
            ProcessAction();
            //
            if(canMove)
            {
                bodyMovement += lastMove * lastInputNorm * speed * speedFactor;
                Vector3 direction = lastMove;
                if (lastInputNorm == 0.0 || lastStrafe)
                {
                    direction = lastDirection;
                }
                if(lastStrafe && lastDirectionNorm == 0.0f)
                {
                    direction = transform.forward;
                }
                transform.forward = Vector3.Lerp(transform.forward, direction, rotFactor * rotationSpeedFactor * stableTimeRatio);
                lastMove = transform.forward;
                lastDirection = transform.forward;
            }

            Vector3 groundMovement = bodyMovement;
            groundMovement.y = 0.0f;
            AnimatorSetBool("Walking", groundMovement.magnitude > 0.3f);
            AnimatorSetFloat("WalkingSpeed", groundMovement.magnitude);
            AnimatorSetFloat("DirectionXForward", Vector3.Dot(groundMovement, transform.forward));
            AnimatorSetFloat("DirectionXRight", Vector3.Dot(groundMovement, transform.right));

            // Update CC
            //cc.Move(movement * Time.deltaTime);
            AnimatorSetFloat("Health", health.GetHealth());

            AnimatorSetBool("Bloom", superMode);

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

            // Apply GameEffects
            activeGameEffects.RemoveAll(item => item == null);
            for (int idx = 0; idx < activeGameEffects.Count; idx++)
            {
                GameEffect ge = activeGameEffects[idx];
                ge.ProcessOnCollision(this, rb, ref bodyMovement);
            }
        }

        public void Move(Request _req)
        {
            if(health.IsDead())
            {
                return;
            }
            if (_req.direction.magnitude > 0.0f && _req.action != Action.ATTACK && _req.action != Action.SPE_ATTACK)
            {
                lastDirection = _req.direction.normalized;
            }
            if (_req.move.magnitude > 0.0f)
            {
                lastMove = _req.move.normalized;
            }
            lastInputNorm = _req.inputNorm;
            lastDirectionNorm = _req.directionNorm;
            lastStrafe = _req.strafe;

            superMode = _req.modifier;

            if (_req.action != Action.NONE)
            {
                action = _req.action;
                if(action == Action.ATTACK
                    && comboAttackLevel < Constants.COMBO_STRIKE_MAX
                    && (comboAttackLevel == 0
                        || ((frame - comboAttackLastFrame) > Constants.COMBO_STRIKE_FRAME_INF 
                            && (frame - comboAttackLastFrame) <= Constants.COMBO_STRIKE_FRAME_SUP)))
                {
                    comboAttackLastFrame = frame;
                    comboAttackLevel++;
                    Debug.Log("Combo " + comboAttackLevel);
                } else if(action != Action.ATTACK_AIM)
                {
                    Debug.Log((frame - comboAttackLastFrame));
                    comboAttackLastFrame = 0;
                    comboAttackLevel = 0;
                }
            }
        }

        private void ProcessAction()
        {
            if (!blockActions)
            {
                switch (action)
                {
                    case Action.ATTACK:
                        AnimatorSetBool("Attack", true);
                        AnimatorSetBool("AttackAim", false);

                        if(comboAttackLevel == 2)
                        {
                            AnimatorSetBool("Attack2", true);
                        }
                        if (comboAttackLevel == 3)
                        {
                            AnimatorSetBool("Attack3", true);
                        }
                        break;
                    case Action.SPE_ATTACK:
                    case Action.SPE_DEFENSE:
                    case Action.SPE_MOVEMENT:
                    case Action.SPE_SPECIAL:
                    case Action.MOVEMENT:
                    case Action.DEFENSE:
                    case Action.SPECIAL:
                        TriggerGameEffect(action);
                        AnimatorSetBool("AttackAim", false);
                        AnimatorSetBool("SpeAttack", true);
                        AnimatorSetBool("SpeDefenseAim", false);
                        break;
                    case Action.ATTACK_AIM:
                        AnimatorSetBool("AttackAim", true);
                        break;
                    case Action.SPE_ATTACK_AIM:
                        AnimatorSetBool("SpeAttackAim", true);
                        AnimatorSetBool("SpeAttack", false);
                        break;
                    case Action.SPE_DEFENSE_AIM:
                        AnimatorSetBool("SpeDefenseAim", true);
                        AnimatorSetBool("SpeDefense", false);
                        break;
                }
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

            ExecuteCommand(command);
        }

        private void ExecuteCommand(GameEffectCommand command)
        {
            if (command.toInstanciate == null)
                return;

            if (command.timer > 0.0f)
            {
                Debug.Log("Too soon");
                return;
            }

            command.timer = command.cooldown;

            if(command.unique && command.previous != null)
            {
                return;
            }

            GameObject geGameObject = Instantiate(command.toInstanciate);
            GameEffect gameEffect = geGameObject.GetComponent<GameEffect>();
            List<GameEffect> effects = new List<GameEffect>();
            if (gameEffect == null)
            {
                effects.AddRange(geGameObject.GetComponentsInChildren<GameEffect>());
            } else
            {
                effects.Add(gameEffect);
            }

            foreach (GameEffect ge in effects)
            {
                ge.SetOwner(this);
            }

            geGameObject.transform.position = transform.position;
            geGameObject.transform.forward = transform.forward;
            if (command.child)
            {
                if(gameEffect != null)
                    gameEffect.FollowTransform(transform, true, false);
                foreach (GameEffect ge in effects)
                {
                    if (ge == gameEffect)
                        continue;
                    ge.FollowTransform(transform, true, false, true);
                }
            }
            command.previous = geGameObject;
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
            rb.drag = 20.0f;
            GameManager.Instance.OnDeath(this);

            rb.constraints = RigidbodyConstraints.FreezeAll;
            col.enabled = false;

            activeGameEffects.Clear();

            if (destroyOnDie)
            {
                Destroy(gameObject);
            }

            if (finishPrefab != null)
            {
                GameObject finishObject = GameObject.Instantiate(finishPrefab);
                finishObject.transform.position = transform.position;
                finishObject.transform.rotation = transform.rotation;
            }
        }

        public void Revive()
        {
            health.SetHealth(Constants.CHARACTER_START_HEALTH);
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.drag = 0.0f;
            col.enabled = true;
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

        public bool IsPlayer
        {
            get
            {
                return isPlayer;
            }
        }

        public void SetWeaponActive(int _intValue)
        {
            weaponIsActive = _intValue > 0;
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
            if (!GameManager.Instance.allowRevive)
                return 0;

            int ret = 0;

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

        public CharacterHealth GetHealth()
        {
            return health;
        }

        public void SetParalyzed(bool _paralyzed)
        {
            canMove = !_paralyzed;
        }

        public void PostAudioEvent(string _eventName)
        {
            SoundManager.Instance.PostEvent(_eventName, this.gameObject);
        }

        public void ExecuteAdditionnalCommand(int _index)
        {
            ExecuteCommand(additionnalCommands[_index]);
        }

        public void ResetAttackBool()
        {
            AnimatorSetBool("Attack", false);
            AnimatorSetBool("Attack2", false);
            AnimatorSetBool("Attack3", false);
            AnimatorSetBool("AttackAim", false);
            AnimatorSetBool("SpeAttackAim", false);
            AnimatorSetBool("SpeAttack", false);
        }

        private void AnimatorSetBool(string _name, bool _value)
        {
            animator.SetBool(_name, _value);
            if (hudHelper != null)
            {
                hudHelper.hudAnimator.SetBool(_name, _value);
            }
            foreach (Animator a in additionalAnimators)
            {
                a.SetBool(_name, _value);
            }
        }

        private void AnimatorSetFloat(string _name, float _value)
        {
            animator.SetFloat(_name, _value);
            if (hudHelper != null)
            {
                hudHelper.hudAnimator.SetFloat(_name, _value);
            }
            foreach (Animator a in additionalAnimators)
            {
                a.SetFloat(_name, _value);
            }
        }

        private void AnimatorSetTrigger(string _name)
        {
            animator.SetTrigger(_name);
            if (hudHelper != null)
            {
                hudHelper.hudAnimator.SetTrigger(_name);
            }
            foreach (Animator a in additionalAnimators)
            {
                a.SetTrigger(_name);
            }
        }

        public void SetSpeedFactor(float _value)
        {
            speedFactor = _value;
        }

        public void SetRotFactor(float _value)
        {
            rotFactor = _value;
        }

        public void SetBlockActions(bool _value)
        {
            blockActions = _value;
        }

        public void SetGravity(bool _value)
        {
            rb.useGravity = _value;
        }

        public void MultiplyVelocity(float _factor)
        {
            rb.velocity *= _factor;
        }

        public float GetRadius()
        {
            return radius;
        }

        public bool IsStrafe()
        {
            return lastStrafe;
        }
    }
}