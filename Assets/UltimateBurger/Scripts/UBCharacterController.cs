﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vbg;

namespace ub
{
    public class UBCharacterController : VBGCharacterController
    {
        public struct Request
        {
            public float LThruster;
            public float RThruster;

            public bool RBrake;
            public bool LBrake;

            public bool attack;
            public bool RDodge;
            public bool LDodge;
        }

        public int playerID;
        private UBUserInput inputs;
        public float maxThruster = 50;
        public float maxBrake = 10;
        public float shift = 0.5f;
        public float speedToShift= 0.01f;

        private Vector3 thrustDirection = new Vector3(0, 0, 1);

        public Transform emissionL;
        public Transform emissionR;
        public GameObject emissionPrefab;

        public List<GameObject> hp;

        // Use this for initialization
        protected void Start()
        {
            base.Start();
            inputs = GetComponent<UBUserInput>();
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            ProcessCooldown(attack);
            ProcessCooldown(defense);
            ProcessCooldown(movement);
        }

        void FixedUpdate()
        {
            if (!UBSceneController.Instance.enableMoves)
                return;
            if (IsDead())
                return;

            Request req = inputs.GetRequest();

            float shiftFactor = Mathf.Abs(shift);

            //Debug.Log("R " + transform.up * shiftFactor * -req.RThruster);
            //Debug.Log("L " + transform.up * shiftFactor * req.LThruster);

            float rBrakeEffect = maxBrake * (req.RBrake ? 1.0f : 0);
            float lBrakeEffect = maxBrake * (req.LBrake ? 1.0f : 0);

            rb.AddRelativeTorque(transform.up * shiftFactor * -(req.RThruster - rBrakeEffect), ForceMode.VelocityChange);
            rb.AddRelativeTorque(transform.up * shiftFactor * (req.LThruster - lBrakeEffect), ForceMode.VelocityChange);

            //Debug.Log(rb.angularVelocity);

            rb.AddForce((req.LThruster + req.RThruster - rBrakeEffect - lBrakeEffect) * maxThruster * TransformToWorld(thrustDirection));

            if (req.LThruster - lBrakeEffect > 0.0f)
            {
                GameObject go = Instantiate(emissionPrefab);
                go.transform.position = emissionL.position;
                go.transform.rotation = emissionL.rotation;
            }
            if (req.RThruster - rBrakeEffect > 0.0f)
            {
                GameObject go = Instantiate(emissionPrefab);
                go.transform.position = emissionR.position;
                go.transform.rotation = emissionR.rotation;
            }

            if (req.attack)
            {
                ExecuteCommand(attack);
            }


            if (req.RDodge)
            {
                ExecuteCommand(defense);
            }
            if (req.LDodge)
            {
                ExecuteCommand(movement);
            }
            /*
            RaycastHit hit;
            Ray groundRay = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(groundRay, out hit, groundCheckDist + 0.15f, Ground, QueryTriggerInteraction.Ignore))
            {
                Transform ground = hit.collider.gameObject.transform;
                Debug.Log(ground);
                transform.up = ground.up;
            }
            */

            // Apply GameEffects
            activeGameEffects.RemoveAll(item => item == null);
            for (int idx = 0; idx < activeGameEffects.Count; idx++)
            {
                GameEffect ge = activeGameEffects[idx];
                ge.ProcessOnCollision(this, rb, ref bodyMovement);
            }
        }

        public override void Damage(float intensity)
        {
            base.Damage(intensity);
            hp[hp.Count - 1].SetActive(false);
            hp.RemoveAt(hp.Count - 1);
        }

        Vector3 TransformToWorld(Vector3 from)
        {
            return from.x * transform.right
                + from.y * transform.up
                + from.z * transform.forward;
        }
        /*
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.layer == Ground)
            {
                Transform ground = collision.collider.transform;
                Debug.Log(ground);
                transform.up = ground.up;
            }
        }
        */


        private bool ExecuteCommand(VBGCharacterController.GameEffectCommand command)
        {
            if (command.toInstanciate == null)
                return false;

            if (command.timer > 0.0f)
            {
                Debug.Log("Too soon");
                return false;
            }

            command.timer = command.cooldown;

            if (command.unique && command.previous != null)
            {
                return false;
            }

            GameObject geGameObject = Instantiate(command.toInstanciate, command.trueChild ? transform : null);
            GameEffect gameEffect = geGameObject.GetComponent<GameEffect>();
            List<GameEffect> effects = new List<GameEffect>();
            if (gameEffect == null)
            {
                effects.AddRange(geGameObject.GetComponentsInChildren<GameEffect>());
            }
            else
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
                if (gameEffect != null)
                    gameEffect.FollowTransform(transform, true, false);
                foreach (GameEffect ge in effects)
                {
                    if (ge == gameEffect)
                        continue;
                    ge.FollowTransform(transform, true, false, true);
                }
            }
            command.previous = geGameObject;
            return true;
        }
    }
}