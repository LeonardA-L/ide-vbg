using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
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
        public GameObject finishPrefab;
        public Vector3 initialVelocity;
        public VBGCharacterController owner;
        public OwnerActive ownerActive = OwnerActive.NO;
        public bool destroyParent = false;
        public ProcessMode processMode = ProcessMode.ON_COLLISION;
        [Header("Health impact")]
        public float healthImpact = 0.0f;
        public bool impactPerFrame = false;
        public bool friendlyFire = false;
        [Header("Push Force")]
        public Vector3 pushForceVector;
        public float pushForceNorm = 0.0f;
        public bool pushForceIsOmnidirectional;
        public bool pushForceNoY = true;
        public float pushForceDecreaseLength = 0.0f;
        [Header("Switch")]
        public string switchName;
        public bool switchValue = false;
        public bool unstable = false;

        GameEffectExit[] exitConditions;
        GameEffectActivate[] activateConditions;
        List<VBGCharacterController> impactedCharacters;
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
            impactedCharacters = new List<VBGCharacterController>();

            rb = GetComponent<Rigidbody>();
            if(initialVelocity.magnitude > 0.0f && rb != null)
            {
                rb.velocity = initialVelocity;
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
                transform.position += initialVelocity * Time.deltaTime;
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
            foreach (VBGCharacterController cc in impactedCharacters)
            {
                cc.UnRegisterGameEffect(this);
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

        private void OnTriggerEnter(UnityEngine.Collider other)
        {


            if (other.gameObject.tag == GameManager.Constants.TAG_CHARACTER
                || other.gameObject.tag == GameManager.Constants.TAG_NONPLAYER_CHARACTER)
            {
                VBGCharacterController cc = other.gameObject.GetComponent<VBGCharacterController>();
                Debug.Assert(cc != null);

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

                RegisterCharacter(cc);
                cc.RegisterGameEffect(this);
                // TODO process here ?
            }
        }

        private void OnTriggerExit(UnityEngine.Collider other)
        {


            if (other.gameObject.tag == GameManager.Constants.TAG_CHARACTER
                || other.gameObject.tag == GameManager.Constants.TAG_NONPLAYER_CHARACTER)
            {
                VBGCharacterController cc = other.gameObject.GetComponent<VBGCharacterController>();
                Debug.Assert(cc != null);

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

                UnRegisterCharacter(cc);
                cc.UnRegisterGameEffect(this);
            }
        }

        public void RegisterCharacter(VBGCharacterController cc)
        {
            impactedCharacters.Add(cc);
        }

        public void UnRegisterCharacter(VBGCharacterController cc)
        {
            impactedCharacters.Remove(cc);
        }

        private void ProcessHealth(VBGCharacterController cc)
        {
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

        private void ProcessPushForce(VBGCharacterController cc, Rigidbody rb, ref Vector3 characterMovement)
        {
            if (pushForceNorm > 0.0f)
            {
                Vector3 force = new Vector3();
                float forceNorm = pushForceNorm;
                if (pushForceDecreaseLength > 0.0f)
                {
                    float distRatio = (cc.transform.position - transform.position).magnitude / pushForceDecreaseLength;
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
                    Vector3 movement = (cc.transform.position - transform.position);
                    if (pushForceNoY)
                    {
                        movement.y = 0.0f;
                    }
                    movement.Normalize();
                    force += movement * forceNorm;
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

        public void ProcessOnCollision(VBGCharacterController cc, Rigidbody rb, ref Vector3 characterMovement)
        {
            Debug.Log("Process On Collision " + gameObject.name);

            if(!IsActive(cc))
            {
                return;
            }

            lastFrameProcessed = true;

            ProcessPushForce(cc, rb, ref characterMovement);
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