using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace vbg
{
    [AddComponentMenu("GameEffect/New Game Effect")]
    public class GameEffect : MonoBehaviour
    {
        [System.Serializable]
        public class VibrationImpact
        {
            public bool active = false;
            [Range(0,1)]
            public float force = 1.0f;
            public float duration = 1.0f;
            [Range(0,1)]
            public float lerp = 1.0f;
        }

        [System.Serializable]
        public class PushForce
        {
            [Tooltip("Is Push Force Active")]
            public bool active = false;
            [Tooltip("Force that impacts the character that takes the game effect. Normalized")]
            public Vector3 pushForceVector;
            [Tooltip("Norm of the push force vector")]
            public float pushForceNorm = 0.0f;
            [Tooltip("The force pushes the activator away instead of using pushForceVector")]
            public bool pushForceIsOmnidirectional;
            [Tooltip("use the owner's transform as reference instead of the effect's transform")]
            public bool ownerAsReference = false;
            [Tooltip("Should the push force lift the player or not")]
            public bool pushForceNoY = true;
            [Tooltip("Ratio for the decrease from the center of the GameEffect")]
            public float pushForceDecreaseLength = 0.0f;
            //[Tooltip("Resets the momentum of the target when it is first activated")]
            //public bool resetMomentum = false;
            public bool asVelocity = false;
        }

        [System.Serializable]
        public class HealthImpact
        {
            [Tooltip("Is Health Impact Active")]
            public bool active = false;
            [Tooltip("How much health difference the character will get (negative: dammage, positive: heal)")]
            public float impact = 0.0f;
            [Tooltip("Is the health impact per frame or per second")]
            public bool impactPerFrame = false;
            [Tooltip("Is friendly fire enabled")]
            public bool friendlyFire = false;
        }

        [System.Serializable]
        public class SwitchImpact
        {
            [Tooltip("Is Switch Impact Active")]
            public bool active = false;
            [Tooltip("Name of the switch the GameEffect will toggle")]
            public string name;
            [Tooltip("Value of the switch the GameEffect will set when processed (in most cases, true)")]
            public bool newValue = true;
            [Tooltip("Should the GameEffect toggle the switch back when it's not being processed")]
            public bool unstable = false;
        }

        [System.Serializable]
        public class ValueImpact
        {
            [Tooltip("Is Value Impact Active")]
            public bool active = false;
            [Tooltip("Name of the value the GameEffect will update")]
            public string name;
            [Tooltip("How should the value be updated (NONE, ABSOLUTE, RELATIVE)")]
            public FloatValueMode updateMode;
            [Tooltip("Absolute or Relative value the GameEffect will set when triggered")]
            public float update = 0;
            [Tooltip("Should the GameEffect revert the update to the value when it's not being processed (only in ONCE mode)")]
            public bool unstable = false;
            [Tooltip("Rate at which the value is updated")]
            public FloatValueUpdate updateRate = FloatValueUpdate.ONCE;
        }

        [System.Serializable]
        public class Teleport
        {
            [Tooltip("Is Teleport Active")]
            public bool active = false;
            public bool toNearestSpawnPoint = false;
            [Tooltip("Hotspot to teleport the object to")]
            public Transform toHotspot;
            [Tooltip("Use hotspot rotation when teleporting")]
            public bool useHotspotRotation;
            [Tooltip("Preserve momentum of the object when teleporting")]
            public bool preserveMomentum = false;
        }

        [System.Serializable]
        public class AnimatorImpact
        {
            [Tooltip("Is Animator Impact Active")]
            public bool active = false;
            [Tooltip("Animator to impact. If none, will try to fetch the owner's animator")]
            public Animator animator;

            [Tooltip("Name of the trigger parameter to update in the animator")]
            public string triggerName;

            [Tooltip("Name of the Boolean parameter to activate while the effect is processed")]
            public string boolName;
            [Tooltip("Value for the boolean to set")]
            public bool boolValue = true;

            public bool unstable = false;
        }

        [System.Serializable]
        public class OwnerImpact
        {
            [Tooltip("Is Owner Impact Active")]
            public bool active = false;
            [Tooltip("The effect will paralyse its owner for the time it's activated")]
            public bool paralyse;
            [Tooltip("The effect will add a multiplier to its owner's speed for the time it's activated")]
            public float speedFactor = 1.0f;
            [Tooltip("The effect will add a multiplier to its owner's rotation speed for the time it's activated")]
            public float rotFactor = 1.0f;
            [Tooltip("The effect will block all actions (jump, attack, skills) on its owner for the time it's activated")]
            public bool blockActions = false;
            public bool disableGravity = false;
            [Tooltip("Multiply the rigidbody's velocity when the effect ends")]
            public float velocityEndFactor = 1.0f;
            public bool enableDefenseMode = true;
            public VibrationImpact vibration;
        }

        [System.Serializable]
        public class CreatureImpact
        {
            [Tooltip("Is Creature Impact Active")]
            public bool active = false;
            [Tooltip("Animator to impact. If none, will try to fetch the owner's animator")]
            public GameObject addGameEffect;
            public bool asOwner = true;
            public VibrationImpact vibration;
        }

        [System.Serializable]
        public class AudioImpact
        {
            [Tooltip("Is Audio Impact Active")]
            public bool active = false;
            public string startEvent;
            public string endEvent;
            public bool useActivatorTransform = false;
            public bool useOwnerTransform = false;
            public List<string> switchNames = new List<string>();
            public List<string> switchValues = new List<string>();
        }

        [System.Serializable]
        public class ScriptImpact
        {
            [Tooltip("Is Script Impact Active")]
            public bool active = false;
            public UnityEvent action;
        }

        [System.Serializable]
        public class ActiveImpact
        {
            [Tooltip("Is Active Impact Active")]
            public bool active = false;
            public List<GameObject> objects;
            public bool activate = true;
            public bool destroyActivator = false;
        }

        public enum FloatValueMode
        {
            NONE,
            ABSOLUTE,
            RELATIVE
        }

        public enum FloatValueUpdate
        {
            ONCE,
            PER_FRAME,
            PER_SECOND
        }

        // Parameters
        [Tooltip("A prefab to instantiate when the game effect finishes")]
        public GameObject finishPrefab;
        [Tooltip("The initial velocity (or force if it has a RigidBody) of the object")]
        public Vector3 initialVelocity;
        public bool initialVelocityOwnerReference = false;
        [Tooltip("Owner the object belongs to. This field is filled automatically if the GE is an action")]
        public VBGCharacterController owner;
        [Tooltip("Should the game effect destroy its parent when it dies (is it embedded)")]
        public bool destroyParent = false;
        public bool destroyParentParent = false;
        [Tooltip("Should the game effect destroy itself when it encounters a collision")]
        public bool destroyOnWall = false;
        //[Tooltip("Should the game effect process every frame or only when colliding with a character")]
        //public ProcessMode processMode = ProcessMode.ON_COLLISION;
        [Tooltip("When the game effect finishes, it will be reset instead of destroyed")]
        public bool resetNotFinish = false;
        [Tooltip("When checked, the game effect will remain activated even if its activation conditions fail")]
        public bool stable = false;
        [Tooltip("When checked, the effect will be processed once per activator per activation cycle only")]
        public bool activateOncePerActivator = false;

        [Header("Impacts")]
        public HealthImpact healthImpact;
        public PushForce pushForce;
        public SwitchImpact switchImpact;
        public ValueImpact valueImpact;
        public Teleport teleport;
        public AnimatorImpact animatorImpact;
        public OwnerImpact ownerImpact;
        public CreatureImpact creatureImpact;
        public AudioImpact audioImpact;
        public ScriptImpact scriptImpact;
        public ActiveImpact activeImpact;

        private bool hasValueBeenUpdated = false;

        GameEffectExit[] exitConditions;
        GameEffectActivate[] activateConditions;
        List<IDynamic> impactedCharacters = new List<IDynamic>();
        List<IDynamic> activators = new List<IDynamic>();
        private bool toDelete = false;
        private bool finished = false;
        private Transform toFollow;
        private Vector3 followOffset;
        private bool followForward;
        private bool followRotation;
        private bool processedOnce = false;
        private bool processedOnceThisCycle = false;
        private bool processedOnceThisFixedCycle = false;

        private bool lastFrameProcessed = false;
        private int lastFixedFrameProcessed = 0;

        private Transform lastSoundActivator;

        Rigidbody rb;

        // Use this for initialization
        void Start()
        {
            exitConditions = GetComponents<GameEffectExit>();
            activateConditions = GetComponents<GameEffectActivate>();

            rb = GetComponent<Rigidbody>();
            if(initialVelocity.magnitude > 0.0f && rb != null && rb.useGravity)
            {
                Transform refTransform = initialVelocityOwnerReference ? owner.transform : transform;
                rb.velocity = (initialVelocity.x * refTransform.right
                             + initialVelocity.y * refTransform.up
                             + initialVelocity.z * refTransform.forward);
            }
        }

        private void FixedUpdate()
        {
            lastFixedFrameProcessed++;
        }

        // Update is called once per frame
        void Update()
        {
            Unstables();

            if (toDelete)
            {
                Finish();
            }
            foreach (GameEffectExit gee in exitConditions)
            {
                if (gee.AfterUpdate())
                {
                    toDelete = true;
                    break;
                }
            }

            if (toFollow != null)
            {
                transform.position = toFollow.position;

                if (followForward)
                {
                    transform.forward = toFollow.forward;
                }
                else if (followRotation)
                {
                    transform.rotation = toFollow.rotation;
                }
                transform.position += followOffset.x * transform.right
                                    + followOffset.y * transform.up
                                    + followOffset.z * transform.forward;
            }

            if (initialVelocity.magnitude > 0.0f && rb == null)
            {
                Transform refTransform = initialVelocityOwnerReference ? owner.transform : transform;
                transform.position += (initialVelocity.x * refTransform.right
                                     + initialVelocity.y * refTransform.up
                                     + initialVelocity.z * refTransform.forward)
                                     * Time.deltaTime;
            }
            if (initialVelocity.magnitude > 0.0f && rb != null && !rb.useGravity)
            {
                Transform refTransform = initialVelocityOwnerReference ? owner.transform : transform;
                rb.velocity = (initialVelocity.x * refTransform.right
                             + initialVelocity.y * refTransform.up
                             + initialVelocity.z * refTransform.forward);
            }


            if (!IsActive(null))
            {
                if(!lastFrameProcessed)
                {
                    processedOnceThisCycle = false;
                }
                lastFrameProcessed = false;
                return;
            }

            ProcessAlways();
        }

        public void Finish(bool destroy = false)
        {
            //Debug.Log("Finish " + name);

            Unstables(true);
            finished = true;

            if (ownerImpact.active && owner != null)
            {
                owner.SetParalyzed(false);
                owner.SetSpeedFactor(1.0f);
                owner.SetRotFactor(1.0f);
                owner.SetBlockActions(false);
                owner.SetGravity(true);
                owner.MultiplyVelocity(ownerImpact.velocityEndFactor);
                if(ownerImpact.enableDefenseMode)
                {
                    owner.SetDefenseMode(false);
                }
            }

            impactedCharacters.RemoveAll(item => item == null);
            foreach (IDynamic dy in impactedCharacters)
            {
                dy.UnRegisterGameEffect(this);
            }

            if (!resetNotFinish)
            {
                if (destroyParent)
                {
                    GameObject.Destroy(transform.parent.gameObject);
                }
                if(destroyParentParent)
                {
                    GameObject.Destroy(transform.parent.parent.gameObject);
                }
                GameObject.Destroy(gameObject);
            }
            else
            {
                Reset();
            }

            if (finishPrefab != null)
            {
                GameObject finishObject = GameObject.Instantiate(finishPrefab);
                finishObject.transform.position = transform.position;
                finishObject.transform.rotation = transform.rotation;

                GameEffect ge = finishObject.GetComponent<GameEffect>();
                if (ge != null) {
                    ge.SetOwner(owner);
                }
            }

            toDelete = false;
        }

        private void Unstables(bool finish = false, bool destroy = false)
        {
            if (switchImpact.unstable && !lastFrameProcessed && lastFixedFrameProcessed > 1)
            {
                SwitchManager.Instance.SetSwitch(switchImpact.name, !switchImpact.newValue);
            }

            if (valueImpact.unstable && !lastFrameProcessed && lastFixedFrameProcessed > 1 && hasValueBeenUpdated && valueImpact.updateRate == FloatValueUpdate.ONCE)
            {
                SwitchManager.Instance.SetValue(valueImpact.name, (float)SwitchManager.Instance.GetValue(valueImpact.name) - valueImpact.update);
                hasValueBeenUpdated = false;
            }

            if (
                (
                    (!animatorImpact.unstable && !lastFrameProcessed)
                    || finish
                    || (animatorImpact.unstable && lastFixedFrameProcessed > 1)
                )
                && animatorImpact.boolName != null && animatorImpact.boolName != "")
            {
                Animator animator = animatorImpact.animator;

                if (animator == null && owner != null)
                {
                    animator = owner.GetComponent<Animator>();
                }

                animator.SetBool(animatorImpact.boolName, !animatorImpact.boolValue);
            }

            if(destroy)
            {
                return;
            }

            if(
                finish
                || (lastFixedFrameProcessed > 1 && processedOnceThisFixedCycle)
                || (!lastFrameProcessed && processedOnceThisCycle)
                )
            {
                if (audioImpact.active && audioImpact.endEvent != null && audioImpact.endEvent != "")
                {
                    //Debug.Log(lastFixedFrameProcessed);
                    //Debug.Log("Last "+lastSoundActivator);
                    //Debug.Log("parent "+ transform.parent.gameObject);
                    GameObject audioGo = (audioImpact.useOwnerTransform ? owner.transform : (audioImpact.useActivatorTransform ? lastSoundActivator : transform.parent ?? transform)).gameObject;
                    //Debug.Log("parent "+ transform.parent.gameObject);
                    SoundManager.Instance.PostEvent(audioImpact.endEvent, audioGo);
                }
            }



            if (lastFixedFrameProcessed > 1)
            {
                processedOnceThisFixedCycle = false;
                activators.Clear();
            }
        }

        private void Reset()
        {
            Unstables();
            foreach (GameEffectExit gee in exitConditions)
            {
                gee.Reset();
            }
            foreach (GameEffectActivate gea in activateConditions)
            {
                gea.Reset();
            }

            processedOnce = false;
            toDelete = false;
            finished = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnTriggerEnter(collision.collider);
        }

        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            TriggerGameEffect(other.gameObject);
        }

        public void TriggerGameEffect(GameObject other)
        {
            //Debug.Log(this.gameObject.name + " " +other.name);

            if (other.tag == GameManager.Constants.TAG_PLAYER
                || other.tag == GameManager.Constants.TAG_DYNAMIC
                || other.tag == GameManager.Constants.TAG_ENNEMY)
            {
                VBGCharacterController cc = other.GetComponent<VBGCharacterController>();
                Dynamic dy = other.GetComponent<Dynamic>();
                if (cc == null && dy == null)
                {
                    return;
                }

                IDynamic idy = cc ?? (IDynamic)dy;
                /*if (!IsActive(idy))
                {
                    return;
                }*/

                if (cc != null)
                {
                    RegisterDynamic(cc);
                    cc.RegisterGameEffect(this);
                }
                else
                {
                    UnRegisterDynamic(dy);
                    dy.RegisterGameEffect(this);
                }
                // TODO process here ?
            }
            else if (destroyOnWall && other.layer != GameManager.Constants.LAYER_SOUND && other.tag != GameManager.Constants.TAG_NONTRIGGERCOLLIDER)
            {
                //Debug.Log("Onwall " + name + " - " +other.name);
                if(other.tag == GameManager.Constants.TAG_GAMEEFFECT)
                {
                    GameEffect otherGE = other.GetComponent<GameEffect>();
                    Vector3 v3 = new Vector3();
                    otherGE.ProcessOnCollision(this.GetComponent<Dynamic>(), null, ref v3);
                }
                Finish();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            OnTriggerExit(collision.collider);
        }

        private void OnTriggerExit(UnityEngine.Collider other)
        {

            UnTriggerGameEffect(other.gameObject);
        }

        public void UnTriggerGameEffect(GameObject other)
        {
            if (other.tag == GameManager.Constants.TAG_PLAYER
                || other.tag == GameManager.Constants.TAG_DYNAMIC
                || other.tag == GameManager.Constants.TAG_ENNEMY)
            {
                VBGCharacterController cc = other.GetComponent<VBGCharacterController>();
                Dynamic dy = other.GetComponent<Dynamic>();

                if (cc == null && dy == null)
                {
                    Debug.Log("Both Null");
                    return;
                }

                IDynamic idy = cc == null ? (IDynamic)dy : cc;
                if (!IsActive(idy))
                {
                    //return;
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

        private void ProcessHealth(IDynamic idy, string tag)
        {
            if (!healthImpact.active)
                return;

            if(idy == null)
            {
                return;
            }

            if (healthImpact.impact != 0.0f)
            {
                if (owner != null)
                {
                    if(!healthImpact.friendlyFire && owner.tag == tag)
                    {
                        return;
                    }
                }

                if (healthImpact.impact< 0.0f)
                {
                    VBGCharacterController cc = (idy as VBGCharacterController);
                    float defenseFactor = cc != null ? (cc.defenseMode ? cc.defenseModeDeflectFactor : 1.0f) : 1.0f;
                    if(defenseFactor != 1.0f)
                    {
                        cc.ResetDefense(true);
                    }
                    idy.Damage(healthImpact.impact * (healthImpact.impactPerFrame ? 1 : Time.deltaTime) * defenseFactor);
                }
                else
                {
                    idy.Heal(healthImpact.impact * (healthImpact.impactPerFrame ? 1 : Time.deltaTime));
                }
            }
        }

        private void ProcessSwitch()
        {
            if (!switchImpact.active)
                return;
            
            if(switchImpact.name == null || switchImpact.name == "")
            {
                return;
            }

            SwitchManager.Instance.SetSwitch(switchImpact.name, switchImpact.newValue);
        }

        private void ProcessValue()
        {
            if (!valueImpact.active)
                return;

            if (valueImpact.name == null || valueImpact.name == "" || valueImpact.updateMode == FloatValueMode.NONE || (hasValueBeenUpdated && valueImpact.updateRate == FloatValueUpdate.ONCE))
            {
                return;
            }

            float newValue = valueImpact.update * (valueImpact.updateRate == FloatValueUpdate.PER_SECOND ? Time.deltaTime : 1.0f);
            hasValueBeenUpdated = true;

            if(valueImpact.updateMode == FloatValueMode.RELATIVE)
            {
                newValue += (float)SwitchManager.Instance.GetValue(valueImpact.name);
            }

            SwitchManager.Instance.SetValue(valueImpact.name, newValue);
        }

        private void ProcessPushForce(Transform tr, Rigidbody rb, ref Vector3 characterMovement)
        {
            if (!pushForce.active)
                return;

            Debug.Assert(rb != null, "No Rigidbody passed to PushForce");
            /*if (pushForce.resetMomentum && !lastFrameProcessed)
            {
                Debug.Log("Go");
                rb.velocity = new Vector3();
            }*/

            if (pushForce.pushForceNorm > 0.0f)
            {
                Transform refTransform = pushForce.ownerAsReference ? owner.transform : transform;
                Vector3 force = new Vector3();
                float forceNorm = pushForce.pushForceNorm * rb.mass;
                float stableTimeRatio = Time.fixedDeltaTime * GameManager.Constants.FPS_REF;
                if (pushForce.pushForceDecreaseLength > 0.0f)
                {
                    float distRatio = pushForce.pushForceDecreaseLength / (tr.transform.position - refTransform.position).magnitude;
                    forceNorm *= distRatio;
                }
                if (pushForce.pushForceVector.magnitude > 0.0f)
                {
                    Debug.Assert(!pushForce.pushForceIsOmnidirectional, "Directionnal and Omnidirectional pushforce detected.");
                    Vector3 normalizedPF = pushForce.pushForceVector.normalized;
                    force +=  (normalizedPF.x * refTransform.right
                             + normalizedPF.y * refTransform.up
                             + normalizedPF.z * refTransform.forward)
                             * forceNorm
                             * stableTimeRatio;
                }
                else if (pushForce.pushForceIsOmnidirectional)
                {
                    Vector3 movement = (tr.position - refTransform.position);
                    //Debug.DrawLine(tr.position, tr.position + movement * 2, Color.blue, 2.0f);
                    if (pushForce.pushForceNoY)
                    {
                        movement.y = 0.0f;
                    }
                    movement.Normalize();
                    /*force += (movement.x * refTransform.right
                             + movement.y * refTransform.up
                             + movement.z * refTransform.forward)
                             * forceNorm
                             * stableTimeRatio;*/
                    force += movement * forceNorm * stableTimeRatio;
                }

                if (rb != null)
                {
                    if (pushForce.asVelocity)
                    {
                        rb.velocity = force;
                    }
                    else
                    {
                        rb.AddForce(force);
                    }

                } else
                {
                    characterMovement = force;
                }
            }
        }

        public void ProcessTeleport(Transform tr, Rigidbody rb)
        {
            if (!teleport.active)
                return;

            Debug.Assert(rb != null, "No Rigidbody passed to ProcessTeleport");

            if(teleport.toNearestSpawnPoint)
            {
                tr.position = GameManager.Instance.GetNearestSpawnPoint(tr).position;
            }
            else if (teleport.toHotspot)
            {
                tr.position = teleport.toHotspot.position;
            }

            if (!teleport.preserveMomentum)
            {
                rb.velocity = new Vector3();
                rb.angularVelocity = new Vector3();
            }
            if (teleport.useHotspotRotation)
            {
                tr.rotation = teleport.toHotspot.rotation;
            }
        }

        public void ProcessAnimator()
        {
            if (!animatorImpact.active)
                return;

            Animator animator = animatorImpact.animator;

            if(animator == null && owner != null)
            {
                animator = owner.GetComponent<Animator>();
            }

            if(animator == null)
            {
                Debug.Assert(false, "No animator provided nor found");
                return;
            }

            if(animatorImpact.triggerName != "")
                animator.SetTrigger(animatorImpact.triggerName);
            if (animatorImpact.boolName != "")
            {
                //Debug.Log(animator);
                //Debug.Log(animatorImpact.boolName);
                //Debug.Log(animator.GetBool(animatorImpact.boolName));
                animator.SetBool(animatorImpact.boolName, animatorImpact.boolValue);
            }
        }

        public void ProcessOwnerImpact()
        {
            if (owner == null)
                return;

            if (ownerImpact.active)
            {
                owner.SetParalyzed(ownerImpact.paralyse);
                owner.SetSpeedFactor(ownerImpact.speedFactor);
                owner.SetRotFactor(ownerImpact.rotFactor);
                owner.SetBlockActions(ownerImpact.blockActions);
                owner.SetGravity(!ownerImpact.disableGravity);
                if (ownerImpact.enableDefenseMode)
                {
                    owner.SetDefenseMode(true);
                }
            }
            if(ownerImpact.vibration.active)
            {
                CharacterVibrationController cvc = owner.GetComponent<CharacterVibrationController>();
                if(cvc != null)
                {
                    cvc.SetVibration(ownerImpact.vibration.force, ownerImpact.vibration.duration, ownerImpact.vibration.lerp);
                }
            }
        }

        public void ProcessAudioImpact(Transform tr = null)
        {
            if (!audioImpact.active || audioImpact.startEvent == null || audioImpact.startEvent == "" || processedOnceThisCycle || processedOnceThisFixedCycle)
                return;

            GameObject audioGo = (audioImpact.useOwnerTransform ? owner.transform : (audioImpact.useActivatorTransform ? tr : transform.parent ?? transform)).gameObject;
            lastSoundActivator = audioGo.transform;
            for (int i= 0; i < audioImpact.switchNames.Count; i++)
            {
                SoundManager.Instance.SetSwitch(audioImpact.startEvent, audioImpact.switchNames[i], audioImpact.switchValues[i], audioGo);
            }

            SoundManager.Instance.PostEvent(audioImpact.startEvent, audioGo);
        }

        public void ProcessCreatureImpact(Transform tr, VBGCharacterController cc, Rigidbody rb)
        {
            if (!creatureImpact.active)
                return;

            Debug.Assert(rb != null, "No Rigidbody passed to ProcessCreatureImpact");

            if (creatureImpact.addGameEffect != null && tr.Find(creatureImpact.addGameEffect.name + "(Clone)") == null)
            {
                GameObject go = GameObject.Instantiate(creatureImpact.addGameEffect, tr);
                GameEffect ge = go.GetComponent<GameEffect>();

                go.transform.localPosition = new Vector3();

                if(creatureImpact.asOwner)
                {
                    ge.SetOwner(cc);
                }

                // Force trigger of the gameEffect
                ge.TriggerGameEffect(tr.gameObject);
            }

            if(creatureImpact.vibration.active && cc != null)
            {
                CharacterVibrationController cvc = cc.GetComponent<CharacterVibrationController>();
                if (cvc != null)
                {
                    cvc.SetVibration(creatureImpact.vibration.force, creatureImpact.vibration.duration, creatureImpact.vibration.lerp);
                }
            }
        }

        public void ProcessScriptImpact()
        {
            if (!scriptImpact.active)
                return;

            scriptImpact.action.Invoke();
        }

        public void ProcessActiveImpact(GameObject activator)
        {
            if (!activeImpact.active)
                return;

            foreach(GameObject go in activeImpact.objects)
            {
                go.SetActive(activeImpact.activate);
            }

            if (activator == null)
                return;
            if(activeImpact.destroyActivator)
            {
                Destroy(activator);
            }
        }

        private void AfterProcessCommon()
        {
            processedOnce = true;
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
            //Debug.Log("Process Always " + gameObject.name);

            if (finished)
                return;

            if (!IsActive(null))
            {
                lastFrameProcessed = false;
                return;
            }

            ProcessAnimator();
            ProcessOwnerImpact();
            ProcessAudioImpact();
            ProcessScriptImpact();
            ProcessActiveImpact(null);
            ProcessSwitch();
            ProcessValue();
            // Call last
            AfterProcessCommon();
            lastFrameProcessed = true;
            processedOnceThisCycle = true;
        }

        public void ProcessOnCollision(IDynamic idy, Rigidbody rb, ref Vector3 characterMovement)
        {
            if (finished)
                return;

            VBGCharacterController cc = idy as VBGCharacterController;
            Dynamic dy = idy as Dynamic;
            Transform tr = cc != null ? cc.transform : dy.transform;
            GameObject go = cc != null ? cc.gameObject : dy.gameObject;

            if (!IsActive(idy))
            {
                return;
            }

            if(activateOncePerActivator && activators.Contains(idy))
            {
                return;
            }
            if(!activators.Contains(idy))
            {
                activators.Add(idy);
            }

            //Debug.Log("Process On Collision " + gameObject.name + " " + rb.gameObject.name);

            ProcessPushForce(tr, rb, ref characterMovement);
            ProcessHealth(idy, go.tag);
            ProcessTeleport(tr, rb);
            ProcessAnimator();
            ProcessOwnerImpact();
            ProcessCreatureImpact(tr, cc, rb);
            ProcessAudioImpact(tr);
            ProcessScriptImpact();
            ProcessActiveImpact(go);
            ProcessSwitch();
            ProcessValue();
            // Call last
            AfterProcessCommon();
            lastFixedFrameProcessed = 0;
            processedOnceThisFixedCycle = true;
        }

        public VBGCharacterController GetOwner()
        {
            return owner;
        }

        public void SetOwner(VBGCharacterController _owner)
        {
            owner = _owner;
        }

        private bool IsActive(IDynamic idy)
        {
            if(IsStableAndActivated())
            {
                return true;
            }
            foreach (GameEffectActivate gea in activateConditions)
            {
                if (!gea.IsActive(idy))
                {
                    return false;
                }
            }
            return true;
        }

        public void FollowTransform(Transform _transform, bool _followForward, bool _followRotation, bool _preserveOffset = false)
        {
            if(followForward || followRotation)
                Debug.Assert(followForward != followRotation, "FollowForward cannot be equal to FollowRotation");
            followOffset = _preserveOffset ? transform.localPosition : Vector3.zero;
            toFollow = _transform;
            followForward = _followForward;
            followRotation = _followRotation;
        }

        public bool IsStableAndActivated()
        {
            return processedOnce && stable;
        }

        void OnDestroy()
        {
            if(!finished && !GameManager.Instance.isQuitting)
                Finish(true);
        }
    }
}