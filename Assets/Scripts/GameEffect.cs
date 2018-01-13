using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof(Collider))]
    public class GameEffect : MonoBehaviour
    {

        GameEffectExit[] exitConditions;
        List<VBGCharacterController> impactedCharacters;
        private bool toDelete = false;

        // Use this for initialization
        void Start()
        {
            exitConditions = GetComponents<GameEffectExit>();
            impactedCharacters = new List<VBGCharacterController>();
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

            if(toDelete)
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

        public void Process(VBGCharacterController cc)
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
        }
    }
}