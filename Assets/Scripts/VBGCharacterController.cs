﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class VBGCharacterController : MonoBehaviour, IDynamic
    {

        public struct Constants
        {

            public readonly static float CHARACTER_START_HEALTH = 100.0f;
            public readonly static int ANIMATOR_LAYER_ATTACK = 0;
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
        private Vector3 lastDirection;
        private Vector3 lastMove;
        private float lastInputNorm;
        private float lastDirectionNorm;
        private Action action;
        private List<GameEffect> activeGameEffects;
        private bool weaponIsActive;
        private Vector3 bodyMovement;
        public bool isGrounded;


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
            weaponIsActive = animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking");
            // Apply movement
            bodyMovement = rb.velocity;
            ProcessAction();
            //
            //if (isGrounded)
            {
                //if(!attack && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
                {
                    bodyMovement += lastMove * lastInputNorm * speed;
                }
            }
            transform.forward = Vector3.Lerp(transform.forward, lastDirection, rotationSpeedFactor);

            // Apply GameEffects
            activeGameEffects.RemoveAll(item => item == null);
            foreach (GameEffect ge in activeGameEffects)
            {
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
            ProcessCooldown(specialAttack);
            ProcessCooldown(specialDefense);
            ProcessCooldown(specialMovement);
            ProcessCooldown(specialSpecial);

            ProcessCooldown(movement);
            ProcessCooldown(defense);
            ProcessCooldown(special);
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
    }
}