using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof(Collider))]
    public class GameEffect : MonoBehaviour
    {
        // Parameters
        public GameObject finishPrefab;
        public Vector3 initialVelocity;
        [Header("Push Force")]
        public Vector3 pushForceVector;
        public float pushForceNorm = 0.0f;
        public bool pushForceIsOmnidirectional;
        public bool pushForceNoY = true;
        public float pushForceDecreaseLength = 1.0f;

        GameEffectExit[] exitConditions;
        List<VBGCharacterController> impactedCharacters;
        private bool toDelete = false;

        Rigidbody rb;
        Collider col;

        // Use this for initialization
        void Start()
        {
            exitConditions = GetComponents<GameEffectExit>();
            impactedCharacters = new List<VBGCharacterController>();

            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            if(initialVelocity != null && initialVelocity.magnitude > 0.0f && rb != null)
            {
                rb.velocity = initialVelocity;
            }
        }

        // Update is called once per frame
        void Update()
        {
            foreach (GameEffectExit gee in exitConditions)
            {
                if(gee.AfterUpdate())
                {
                    toDelete = true;
                    break;
                }
            }

            if (initialVelocity != null && initialVelocity.magnitude > 0.0f && rb == null)
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
            if (other.gameObject.tag == GameManager.Constants.TAG_CHARACTER)
            {
                VBGCharacterController cc = other.gameObject.GetComponent<VBGCharacterController>();
                Debug.Assert(cc != null);

                RegisterCharacter(cc);
                cc.RegisterGameEffect(this);
                // TODO process here ?
            }
        }

        private void OnTriggerExit(UnityEngine.Collider other)
        {
            if (other.gameObject.tag == GameManager.Constants.TAG_CHARACTER)
            {
                VBGCharacterController cc = other.gameObject.GetComponent<VBGCharacterController>();
                Debug.Assert(cc != null);

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

        public void Process(VBGCharacterController cc, ref Vector3 characterMovement)
        {
            Debug.Log("Process");
            foreach (GameEffectExit gee in exitConditions)
            {
                if (gee.AfterProcess())
                {
                    toDelete = true;
                    break;
                }
            }

            if(pushForceNorm > 0.0f)
            {

                float forceNorm = pushForceNorm;
                if(pushForceDecreaseLength != 1.0f)
                {
                    float distRatio = (cc.transform.position - transform.position).magnitude / pushForceDecreaseLength;
                    forceNorm *= distRatio;
                }
                if (pushForceVector != null && pushForceVector.magnitude > 0.0f)
                {
                    characterMovement += pushForceVector.normalized * forceNorm;
                }
                else if(pushForceIsOmnidirectional)
                {
                    Vector3 movement = (cc.transform.position - transform.position);
                    if(pushForceNoY)
                    {
                        movement.y = 0.0f;
                    }
                    movement.Normalize();
                    characterMovement += movement * forceNorm;
                }
            }
        }
    }
}