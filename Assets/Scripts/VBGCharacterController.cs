using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof(CharacterController))]
    public class VBGCharacterController : MonoBehaviour
    {
        // Components
        private CharacterController cc;

        // Parameters
        private float speed = 2;
        private float rotationSpeedFactor = 0.2f;
        private float jumpHeight = 30.0f;
        private float friction = 0.2f;
        private float airFriction = 0.02f;
        private float gravity = Physics.gravity.y / 3.0f;

        // Members
        private Vector3 lastDirection;
        private float lastInputNorm;
        private bool jump = false;
        private List<GameEffect> activeGameEffects;

        // Use this for initialization
        void Start()
        {
            cc = GetComponent<CharacterController>();
            activeGameEffects = new List<GameEffect>();
        }

        // Update is called once per frame
        void Update()
        {
            // Apply movement
            Vector3 movement = cc.velocity;
            if (cc.isGrounded)
            {
                movement += lastDirection * lastInputNorm * speed;

                // Apply Jump
                if (jump)
                {
                    movement.y += jumpHeight;
                    jump = false;
                }
            }

            transform.forward = Vector3.Lerp(transform.forward, lastDirection, rotationSpeedFactor * lastInputNorm);

            // Apply GameEffects
            activeGameEffects.RemoveAll(item => item == null);
            foreach (GameEffect ge in activeGameEffects)
            {
                ge.Process(this);
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

            // Update CC
            cc.Move(movement * Time.deltaTime);
        }

        public void Move(Vector3 _move, float _inputNorm, bool _jump)
        {
            if (_move.magnitude > 0.0f)
            {
                lastDirection = _move.normalized;
            }
            lastInputNorm = _inputNorm;
            if (_jump)
            {
                jump = true;
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

                RegisterGameEffect(ge);
                ge.RegisterCharacter(this);
                // TODO process here ?
            }
        }
    }
}