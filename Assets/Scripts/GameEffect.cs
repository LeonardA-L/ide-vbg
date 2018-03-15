using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    [AddComponentMenu("GameEffect/New Game Effect")]
    public class GameEffect : MonoBehaviour
    {

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
            [Tooltip("Should the push force lift the player or not")]
            public bool pushForceNoY = true;
            [Tooltip("Ratio for the decrease from the center of the GameEffect")]
            public float pushForceDecreaseLength = 0.0f;
            //[Tooltip("Resets the momentum of the target when it is first activated")]
            //public bool resetMomentum = false;
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
            //public bool automaticNearestSpawnPoint = false;
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
        [Tooltip("Owner the object belongs to. This field is filled automatically if the GE is an action")]
        public VBGCharacterController owner;
        [Tooltip("Should the game effect destroy its parent when it dies (is it embedded)")]
        public bool destroyParent = false;
        [Tooltip("Should the game effect destroy itself when it encounters a collision")]
        public bool destroyOnWall = false;
        //[Tooltip("Should the game effect process every frame or only when colliding with a character")]
        //public ProcessMode processMode = ProcessMode.ON_COLLISION;
        [Tooltip("When the game effect finishes, it will be reset instead of destroyed")]
        public bool resetNotFinish = false;
        [Tooltip("When checked, the game effect will remain activated even if its activation conditions fail")]
        public bool stable = false;

        [Header("Impacts")]
        public HealthImpact healthImpact;
        public PushForce pushForce;
        public SwitchImpact switchImpact;
        public ValueImpact valueImpact;
        public Teleport teleport;
        public AnimatorImpact animatorImpact;


        private bool hasValueBeenUpdated = false;

        GameEffectExit[] exitConditions;
        GameEffectActivate[] activateConditions;
        List<IDynamic> impactedCharacters;
        private IDynamic activator;
        private bool toDelete = false;
        private Transform toFollow;
        private bool followForward;
        private bool followRotation;
        private bool processedOnce = false;

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
            Unstables();

            lastFrameProcessed = false;


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
            }

            if (initialVelocity.magnitude > 0.0f && rb == null)
            {
                transform.position += (initialVelocity.x * transform.right
                                     + initialVelocity.y * transform.up
                                     + initialVelocity.z * transform.forward)
                                     * Time.deltaTime;
            }

            if (!IsActive(null))
            {
                return;
            }

            ProcessAlways();
        }

        void Finish()
        {
            Debug.Log("Finish");
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

        private void Unstables()
        {
            if (switchImpact.unstable && !lastFrameProcessed)
            {
                SwitchManager.Instance.SetSwitch(switchImpact.name, !switchImpact.newValue);
            }

            if (valueImpact.unstable && !lastFrameProcessed && hasValueBeenUpdated && valueImpact.updateRate == FloatValueUpdate.ONCE)
            {
                SwitchManager.Instance.SetValue(valueImpact.name, (float)SwitchManager.Instance.GetValue(valueImpact.name) - valueImpact.update);
                hasValueBeenUpdated = false;
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
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnTriggerEnter(collision.collider);
        }

        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            Debug.Log(other.gameObject.name);

            if (other.gameObject.tag == GameManager.Constants.TAG_PLAYER
                || other.gameObject.tag == GameManager.Constants.TAG_DYNAMIC
                || other.gameObject.tag == GameManager.Constants.TAG_ENNEMY)
            {
                VBGCharacterController cc = other.gameObject.GetComponent<VBGCharacterController>();
                Dynamic dy = other.gameObject.GetComponent<Dynamic>();
                if (cc == null && dy == null)
                {
                    return;
                }

                IDynamic idy = cc ?? (IDynamic)dy;
                activator = idy;
                if (!IsActive(idy))
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
            else if (destroyOnWall && other.gameObject.tag != GameManager.Constants.TAG_NONTRIGGERCOLLIDER)
            {
                Finish();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            OnTriggerExit(collision.collider);
        }

        private void OnTriggerExit(UnityEngine.Collider other)
        {

            if (other.gameObject.tag == GameManager.Constants.TAG_PLAYER
                || other.gameObject.tag == GameManager.Constants.TAG_DYNAMIC
                || other.gameObject.tag == GameManager.Constants.TAG_ENNEMY)
            {
                VBGCharacterController cc = other.gameObject.GetComponent<VBGCharacterController>();
                Dynamic dy = other.gameObject.GetComponent<Dynamic>();

                if (cc == null && dy == null)
                {
                    Debug.Log("Both Null");
                    return;
                }

                IDynamic idy = cc == null ? (IDynamic) dy : cc;
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

                activator = null;
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
                    idy.Damage(healthImpact.impact * (healthImpact.impactPerFrame ? 1 : Time.deltaTime));
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

            /*if (pushForce.resetMomentum && !lastFrameProcessed)
            {
                Debug.Log("Go");
                rb.velocity = new Vector3();
            }*/

            if (pushForce.pushForceNorm > 0.0f)
            {
                Vector3 force = new Vector3();
                float forceNorm = pushForce.pushForceNorm;
                if (pushForce.pushForceDecreaseLength > 0.0f)
                {
                    float distRatio = pushForce.pushForceDecreaseLength / (tr.transform.position - transform.position).magnitude;
                    forceNorm *= distRatio;
                }
                if (pushForce.pushForceVector.magnitude > 0.0f)
                {
                    Vector3 normalizedPF = pushForce.pushForceVector.normalized;
                    force +=  (normalizedPF.x * transform.right
                             + normalizedPF.y * transform.up
                             + normalizedPF.z * transform.forward)
                             * forceNorm;
                }
                else if (pushForce.pushForceIsOmnidirectional)
                {
                    Vector3 movement = (tr.transform.position - transform.position);
                    if (pushForce.pushForceNoY)
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

        public void ProcessTeleport(Transform tr, Rigidbody rb)
        {
            if (!teleport.active)
                return;

            if (teleport.toHotspot)
            {
                tr.position = teleport.toHotspot.position;
                if (!teleport.preserveMomentum)
                {
                    rb.velocity = new Vector3();
                    rb.angularVelocity = new Vector3();
                }
                if(teleport.useHotspotRotation)
                {
                    tr.rotation = teleport.toHotspot.rotation;
                }
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

            animator.SetTrigger(animatorImpact.triggerName);
        }

        private void AfterProcessCommon()
        {
            processedOnce = true;
            lastFrameProcessed = true;
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

            if (!IsActive(null))
            {
                return;
            }

            ProcessSwitch();
            ProcessValue();
            ProcessAnimator();
            // Call last
            AfterProcessCommon();
        }

        public void ProcessOnCollision(IDynamic idy, Rigidbody rb, ref Vector3 characterMovement)
        {
            //Debug.Log("Process On Collision " + gameObject.name);

            VBGCharacterController cc = idy as VBGCharacterController;
            Dynamic dy = idy as Dynamic;
            Transform tr = cc != null ? cc.transform : dy.transform;
            GameObject go = cc != null ? cc.gameObject : dy.gameObject;

            if (!IsActive(idy))
            {
                return;
            }

            ProcessPushForce(tr, rb, ref characterMovement);
            ProcessHealth(idy, go.tag);
            ProcessSwitch();
            ProcessValue();
            ProcessTeleport(tr, rb);
            ProcessAnimator();
            // Call last
            AfterProcessCommon();
        }

        public VBGCharacterController GetOwner()
        {
            return owner;
        }

        public void SetOwner(VBGCharacterController _owner)
        {
            owner = _owner;
        }

        public IDynamic GetActivator()
        {
            return activator;
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

        public void FollowTransform(Transform _transform, bool _followForward, bool _followRotation)
        {
            if(followForward || followRotation)
                Debug.Assert(followForward != followRotation, "FollowForward cannot be equal to FollowRotation");
            toFollow = _transform;
            followForward = _followForward;
            followRotation = _followRotation;
        }

        public bool IsStableAndActivated()
        {
            return processedOnce && stable;
        }
    }
}