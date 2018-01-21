using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class VBGCharacterController : MonoBehaviour
    {

        public struct Constants
        {

            public readonly static float CHARACTER_START_HEALTH = 100.0f;
            public readonly static int ANIMATOR_LAYER_ATTACK = 0;
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

        public struct Request
        {
            public Vector3 move;
            public float inputNorm;
            public Action action;
        }

        // Components
        private Rigidbody rb;
        private CharacterHealth health;
        private Animator animator;

        // Parameters
        public float speed = 6;
        public float rotationSpeedFactor = 0.2f;
        public Transform groundChecker;
        public LayerMask Ground;

        // Members
        private Vector3 lastDirection;
        private float lastInputNorm;
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
            if (isGrounded)
            {
                //if(!attack && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
                {
                    bodyMovement += lastDirection * lastInputNorm * speed;
                }
            }

            transform.forward = Vector3.Lerp(transform.forward, lastDirection, rotationSpeedFactor * lastInputNorm);

            // Apply GameEffects
            activeGameEffects.RemoveAll(item => item == null);
            foreach (GameEffect ge in activeGameEffects)
            {
                ge.Process(this, rb, ref bodyMovement);
            }

            Vector2 groundMovement = bodyMovement;
            groundMovement.y = 0.0f;
            animator.SetBool("Walking", groundMovement.magnitude > 0.3f);

            // Update CC
            //cc.Move(movement * Time.deltaTime);
            animator.SetFloat("Health", health.GetHealth());
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
            if (_req.move.magnitude > 0.0f)
            {
                lastDirection = _req.move.normalized;
            }
            lastInputNorm = _req.inputNorm;

            if(_req.action != Action.NONE)
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
            }

            action = Action.NONE;
        }

        public void RegisterGameEffect(GameEffect ge)
        {
            activeGameEffects.Add(ge);
        }

        public void UnRegisterGameEffect(GameEffect ge)
        {
            activeGameEffects.Remove(ge);
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