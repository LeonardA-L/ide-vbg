﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/New Game Effect")]
    public class GameEffect : MonoBehaviour
    {

        public enum OwnerActive
        {
            NO,
            YES,
            OWNER_ONLY
        }

        public enum ProcessMode
        {
            ON_COLLISION,
            ALWAYS
        }

        // Parameters
        [Tooltip("A prefab to instantiate when the game effect finishes")]
        public GameObject finishPrefab;
        [Tooltip("The initial velocity (or force if it has a RigidBody) of the object")]
        public Vector3 initialVelocity;
        [Tooltip("Owner the object belongs to. This field is filled automatically if the GE is an action")]
        public VBGCharacterController owner;
        [Tooltip("Is the effect active on its owner (in most cases, no) (if there is no owner, everyone is impacted)")]
        public OwnerActive ownerActive = OwnerActive.NO;
        [Tooltip("Should the game effect destroy its parent when it dies (is it embedded)")]
        public bool destroyParent = false;
        [Tooltip("Should the game effect process every frame or only when colliding with a character")]
        public ProcessMode processMode = ProcessMode.ON_COLLISION;
        [Header("Health impact")]
        [Tooltip("How much health difference the character will get (negative: dammage, positive: heal)")]
        public float healthImpact = 0.0f;
        [Tooltip("Is the health impact per frame or per second")]
        public bool impactPerFrame = false;
        [Tooltip("Is friendly fire enabled")]
        public bool friendlyFire = false;
        [Header("Push Force")]
        [Tooltip("Force that impacts the character that takes the game effect. Normalized")]
        public Vector3 pushForceVector;
        [Tooltip("Norm of the push force vector")]
        public float pushForceNorm = 0.0f;
        [Tooltip("The force pushes the activator away instead of using pushForceVector")]
        public bool pushForceIsOmnidirectional;
        [Tooltip("Should the push force lift the player or not")]
        public bool pushForceNoY = true;
        [Tooltip("Ratio for the decrease from the center of the GameEffect")]
        public float pushForceDecreaseLength = 0.0f;
        [Header("Switch")]
        [Tooltip("Name of the switch the GameEffect will toggle")]
        public string switchName;
        [Tooltip("Value of the switch the GameEffect will set when processed (in most cases, true)")]
        public bool switchValue = true;
        [Tooltip("Should the GameEffect toggle the switch back when it's not being processed")]
        public bool unstable = false;

        GameEffectExit[] exitConditions;
        GameEffectActivate[] activateConditions;
        List<IDynamic> impactedCharacters;
        private bool toDelete = false;
        private Transform toFollow;
        private bool followForward;
        private bool followRotation;

        private bool lastFrameProcessed = false;

        Rigidbody rb;

        // Use this for initialization
        void Start()
        {
            exitConditions = GetComponents<GameEffectExit>();
            activateConditions = GetComponents<GameEffectActivate>();
            impactedCharacters = new List<IDynamic>();

            rb = GetComponent<Rigidbody>();
            if(initialVelocity.magnitude > 0.0f && rb != null)
            {
                rb.velocity = (initialVelocity.x * transform.right
                             + initialVelocity.y * transform.up
                             + initialVelocity.z * transform.forward);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsActive(null))
            {
                return;
            }

            if(unstable && !lastFrameProcessed)
            {
                GameManager.Instance.SetSwitch(switchName, !switchValue);
            }

            lastFrameProcessed = false;

            foreach (GameEffectExit gee in exitConditions)
            {
                if(gee.AfterUpdate())
                {
                    toDelete = true;
                    break;
                }
            }

            if (initialVelocity.magnitude > 0.0f && rb == null)
            {
                transform.position += (initialVelocity.x * transform.right
                                     + initialVelocity.y * transform.up
                                     + initialVelocity.z * transform.forward)
                                     * Time.deltaTime;
            }

            if(toFollow != null)
            {
                transform.position = toFollow.position;

                if(followForward)
                {
                    transform.forward = toFollow.forward;
                } else if (followRotation)
                {
                    transform.rotation = toFollow.rotation;
                }
            }

            if (toDelete)
            {
                Finish();
            }

            if(processMode == ProcessMode.ALWAYS)
            {
                ProcessAlways();
            }
        }

        void Finish()
        {
            Debug.Log("Finish");
            impactedCharacters.RemoveAll(item => item == null);
            foreach (IDynamic dy in impactedCharacters)
            {
                dy.UnRegisterGameEffect(this);
            }
            if(destroyParent)
            {
                GameObject.Destroy(transform.parent.gameObject);
            }
            GameObject.Destroy(gameObject);

            if (finishPrefab != null)
            {
                GameObject finishObject = GameObject.Instantiate(finishPrefab);
                finishObject.transform.position = transform.position;
                finishObject.transform.rotation = transform.rotation;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnTriggerEnter(collision.collider);
        }

        private void OnTriggerEnter(UnityEngine.Collider other)
        {

            if (other.gameObject.tag == GameManager.Constants.TAG_CHARACTER
                || other.gameObject.tag == GameManager.Constants.TAG_DYNAMIC)
            {
                VBGCharacterController cc = other.gameObject.GetComponent<VBGCharacterController>();
                Dynamic dy = other.gameObject.GetComponent<Dynamic>();
                if (cc == null && dy == null)
                {
                    return;
                }

                if (!IsActive(cc))
                {
                    return;
                }

                if (ownerActive == OwnerActive.NO && cc != null && cc == owner)
                {
                    return;
                }

                if (ownerActive == OwnerActive.OWNER_ONLY && cc != owner)
                {
                    return;
                }

                if (cc != null)
                {
                    RegisterDynamic(cc);
                    cc.RegisterGameEffect(this);
                } else
                {
                    UnRegisterDynamic(dy);
                    dy.RegisterGameEffect(this);
                }
                // TODO process here ?
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            OnTriggerExit(collision.collider);
        }

        private void OnTriggerExit(UnityEngine.Collider other)
        {


            if (other.gameObject.tag == GameManager.Constants.TAG_CHARACTER
                || other.gameObject.tag == GameManager.Constants.TAG_DYNAMIC)
            {
                VBGCharacterController cc = other.gameObject.GetComponent<VBGCharacterController>();
                Dynamic dy = other.gameObject.GetComponent<Dynamic>();
                if (cc == null && dy == null)
                {
                    Debug.Log("Both Null");
                    return;
                }

                if (!IsActive(cc))
                {
                    return;
                }

                if (ownerActive == OwnerActive.NO && cc == owner)
                {
                    return;
                }

                if (ownerActive == OwnerActive.OWNER_ONLY && cc != owner)
                {
                    return;
                }

                if (cc != null)
                {
                    UnRegisterDynamic(cc);
                    cc.UnRegisterGameEffect(this);
                }
                else
                {
                    UnRegisterDynamic(dy);
                    dy.UnRegisterGameEffect(this);
                }
            }
        }

        public void RegisterDynamic(IDynamic dy)
        {
            impactedCharacters.Add(dy);
        }

        public void UnRegisterDynamic(IDynamic dy)
        {
            impactedCharacters.Remove(dy);
        }

        private void ProcessHealth(VBGCharacterController cc)
        {
            if(cc == null)
            {
                return;
            }

            if (healthImpact != 0.0f)
            {
                if (owner != null)
                {
                    if(!friendlyFire && owner.tag == cc.tag)
                    {
                        return;
                    }
                }
                if (healthImpact < 0.0f)
                {
                    cc.Damage(healthImpact * (impactPerFrame ? 1 : Time.deltaTime));
                }
                else
                {
                    cc.Heal(healthImpact * (impactPerFrame ? 1 : Time.deltaTime));
                }
            }
        }

        private void ProcessSwitch()
        {
            if(switchName == null || switchName == "")
            {
                return;
            }
            GameManager.Instance.SetSwitch(switchName, switchValue);
        }

        private void ProcessPushForce(Transform tr, Rigidbody rb, ref Vector3 characterMovement)
        {
            if (pushForceNorm > 0.0f)
            {
                Vector3 force = new Vector3();
                float forceNorm = pushForceNorm;
                if (pushForceDecreaseLength > 0.0f)
                {
                    float distRatio = pushForceDecreaseLength / (tr.transform.position - transform.position).magnitude;
                    forceNorm *= distRatio;
                }
                if (pushForceVector.magnitude > 0.0f)
                {
                    Vector3 normalizedPF = pushForceVector.normalized;
                    force +=  (normalizedPF.x * transform.right
                             + normalizedPF.y * transform.up
                             + normalizedPF.z * transform.forward)
                             * forceNorm;
                }
                else if (pushForceIsOmnidirectional)
                {
                    Vector3 movement = (tr.transform.position - transform.position);
                    if (pushForceNoY)
                    {
                        movement.y = 0.0f;
                    }
                    movement.Normalize();
                    force += (movement.x * transform.right
                             + movement.y * transform.up
                             + movement.z * transform.forward)
                             * forceNorm;
                }

                if(rb != null)
                {
                    rb.AddForce(force);
                } else
                {
                    characterMovement = force;
                }
            }
        }

        private void AfterProcessCommon()
        {
            foreach (GameEffectExit gee in exitConditions)
            {
                if (gee.AfterProcess())
                {
                    toDelete = true;
                    break;
                }
            }
        }

        public void ProcessAlways()
        {
            Debug.Log("Process Always " + gameObject.name);

            if (!IsActive(null))
            {
                return;
            }

            lastFrameProcessed = true;

            ProcessSwitch();
            // Call last
            AfterProcessCommon();
        }

        public void ProcessOnCollision(IDynamic idy, Rigidbody rb, ref Vector3 characterMovement)
        {
            //Debug.Log("Process On Collision " + gameObject.name);

            VBGCharacterController cc = idy as VBGCharacterController;
            Dynamic dy = idy as Dynamic;
            Transform tr = cc != null ? cc.transform : dy.transform;

            if (!IsActive(cc))
            {
                return;
            }

            lastFrameProcessed = true;

            ProcessPushForce(tr, rb, ref characterMovement);
            ProcessHealth(cc);
            ProcessSwitch();
            // Call last
            AfterProcessCommon();
        }

        public OwnerActive IsOwnerActive()
        {
            return ownerActive;
        }

        public VBGCharacterController GetOwner()
        {
            return owner;
        }

        public void SetOwner(VBGCharacterController _owner)
        {
            owner = _owner;
        }

        private bool IsActive(VBGCharacterController cc)
        {
            foreach (GameEffectActivate gea in activateConditions)
            {
                if (!gea.IsActive(cc))
                {
                    return false;
                }
            }
            return true;
        }

        public void FollowTransform(Transform _transform, bool _followForward, bool _followRotation)
        {
            if(followForward || followRotation)
                Debug.Assert(followForward != followRotation, "FollowForward cannot be equal to FollowRotation");
            toFollow = _transform;
            followForward = _followForward;
            followRotation = _followRotation;
        }
    }
}