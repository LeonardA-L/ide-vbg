using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class VBGCharacterController : MonoBehaviour
    {

        public struct Constants
        {

            public readonly static float CHARACTER_START_HEALTH = 100.0f;
            public readonly static int ANIMATOR_LAYER_ATTACK = 0;
        }

        // Components
        private CharacterController cc;

        // Parameters
        public float speed = 2;
        public float rotationSpeedFactor = 0.2f;
        private float friction = 0.2f;
        private float airFriction = 0.02f;
        private float gravity = Physics.gravity.y / 3.0f;

        // Members
        private Vector3 lastDirection;
        private float lastInputNorm;
        private bool attack = false;
        private List<GameEffect> activeGameEffects;
        private bool weaponIsActive;

        CharacterHealth health;
        Animator animator;

        // Use this for initialization
        void Start()
        {
            cc = GetComponent<CharacterController>();
            activeGameEffects = new List<GameEffect>();
            health = GetComponent<CharacterHealth>();
            animator = GetComponent<Animator>();
            weaponIsActive = false;
        }

        // Update is called once per frame
        void Update()
        {
            weaponIsActive = animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking");
            // Apply movement
            Vector3 movement = cc.velocity;
            if (cc.isGrounded)
            {
                // Apply Jump
                if (attack)
                {
                    animator.SetTrigger("Attack");
                    attack = false;
                }
                //if(!attack && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
                {
                    movement += lastDirection * lastInputNorm * speed;
                }
            }

            transform.forward = Vector3.Lerp(transform.forward, lastDirection, rotationSpeedFactor * lastInputNorm);

            // Apply GameEffects
            activeGameEffects.RemoveAll(item => item == null);
            foreach (GameEffect ge in activeGameEffects)
            {
                ge.Process(this, ref movement);
            }

            // Apply Gravity
            if (!cc.isGrounded)
            {
                movement.y += gravity;
            }

            // Apply friction
            Vector2 groundMovement = new Vector2(movement.x, movement.z);
            groundMovement = Vector2.Lerp(groundMovement, Vector2.zero, cc.isGrounded ? friction : airFriction);
            movement.x = groundMovement.x;
            movement.z = groundMovement.y;

            groundMovement = movement;
            groundMovement.y = 0.0f;
            animator.SetBool("Walking", groundMovement.magnitude > 0.3f);

            // Update CC
            cc.Move(movement * Time.deltaTime);
            animator.SetFloat("Health", health.GetHealth());
        }

        public void Move(Vector3 _move, float _inputNorm, bool _attack)
        {
            if(health.IsDead())
            {
                return;
            }
            if (_move.magnitude > 0.0f)
            {
                lastDirection = _move.normalized;
            }
            lastInputNorm = _inputNorm;
            if (_attack)
            {
                attack = true;
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

        public float GetGravity()
        {
            return gravity;
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.tag == GameManager.Constants.TAG_GAMEEFFECT)
            {
                GameEffect ge = hit.gameObject.GetComponent<GameEffect>();
                Debug.Assert(ge != null);
                VBGCharacterController geOwner = ge.GetOwner();
                if (!ge.IsOwnerActive() && geOwner == this)
                {
                    return;
                }

                RegisterGameEffect(ge);
                ge.RegisterCharacter(this);
                // TODO process here ?
            }
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
    }
}