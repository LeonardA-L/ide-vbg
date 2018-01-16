using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof(UnityEngine.Collider))]
    public class GameEffect : MonoBehaviour
    {
        // Parameters
        public GameObject finishPrefab;
        public Vector3 initialVelocity;
        public bool ownerActive = false;
        [Header("Health impact")]
        public float healthImpact = 0.0f;
        public bool impactPerFrame = false;
        public bool friendlyFire = false;
        public VBGCharacterController owner;
        [Header("Push Force")]
        public Vector3 pushForceVector;
        public float pushForceNorm = 0.0f;
        public bool pushForceIsOmnidirectional;
        public bool pushForceNoY = true;
        public float pushForceDecreaseLength = 0.0f;

        GameEffectExit[] exitConditions;
        GameEffectActivate[] activateConditions;
        List<VBGCharacterController> impactedCharacters;
        private bool toDelete = false;

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

            if (!IsActive())
            {
                return;
            }

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

            if (toDelete)
            {
                Finish();
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

            if (!IsActive())
            {
                return;
            }

            if (other.gameObject.tag == GameManager.Constants.TAG_CHARACTER
                || other.gameObject.tag == GameManager.Constants.TAG_NONPLAYER_CHARACTER)
            {
                VBGCharacterController cc = other.gameObject.GetComponent<VBGCharacterController>();
                Debug.Assert(cc != null);

                if (!ownerActive && cc == owner)
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

            if (!IsActive())
            {
                return;
            }

            if (other.gameObject.tag == GameManager.Constants.TAG_CHARACTER
                || other.gameObject.tag == GameManager.Constants.TAG_NONPLAYER_CHARACTER)
            {
                VBGCharacterController cc = other.gameObject.GetComponent<VBGCharacterController>();
                Debug.Assert(cc != null);

                if (!ownerActive && cc == owner)
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

        private void ProcessPushForce(VBGCharacterController cc, ref Vector3 characterMovement)
        {
            if (pushForceNorm > 0.0f)
            {

                float forceNorm = pushForceNorm;
                if (pushForceDecreaseLength > 0.0f)
                {
                    float distRatio = (cc.transform.position - transform.position).magnitude / pushForceDecreaseLength;
                    forceNorm *= distRatio;
                }
                if (pushForceVector.magnitude > 0.0f)
                {
                    characterMovement += pushForceVector.normalized * forceNorm;
                }
                else if (pushForceIsOmnidirectional)
                {
                    Vector3 movement = (cc.transform.position - transform.position);
                    if (pushForceNoY)
                    {
                        movement.y = 0.0f;
                    }
                    movement.Normalize();
                    characterMovement += movement * forceNorm;
                }
            }
        }

        public void Process(VBGCharacterController cc, ref Vector3 characterMovement)
        {
            Debug.Log("Process");

            if(!IsActive())
            {
                return;
            }

            foreach (GameEffectExit gee in exitConditions)
            {
                if (gee.AfterProcess())
                {
                    toDelete = true;
                    break;
                }
            }

            ProcessPushForce(cc, ref characterMovement);
            ProcessHealth(cc);
        }

        public bool IsOwnerActive()
        {
            return ownerActive;
        }

        public VBGCharacterController GetOwner()
        {
            return owner;
        }

        private bool IsActive()
        {
            foreach (GameEffectActivate gea in activateConditions)
            {
                if (!gea.IsActive())
                {
                    return false;
                }
            }
            return true;
        }
    }
}