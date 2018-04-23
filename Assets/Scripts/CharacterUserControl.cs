using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof (VBGCharacterController))]
    public class CharacterUserControl : MonoBehaviour
    {
        public bool m_aimAttack = false;
        public bool m_aimSpeAttack = false;
        private bool m_aimingAttack = false;
        private bool m_aimingSpeAttack = false;
        public bool m_aimSpeDefense = false;
        private bool m_aimingSpeDefense = false;

        private int controllerID = -1;

        private VBGCharacterController m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private Vector3 m_Dir;

        private float prevModifier;

        string GetControllersuffix()
        {
            return "_P" + controllerID;
        }

        // Use this for initialization
        void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                throw new System.Exception("No camera found");
            }

            m_Character = GetComponent<VBGCharacterController>();
        }

        private void Update()
        {
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if(controllerID < 0)
            {
                return;
            }

            // read inputs
            float h = Input.GetAxis("Horizontal" + GetControllersuffix());
            float v = Input.GetAxis("Vertical" + GetControllersuffix());
            float hr = Input.GetAxis("RHorizontal" + GetControllersuffix());
            float vr = Input.GetAxis("RVertical" + GetControllersuffix());
            Vector2 joy = new Vector2(h, v);
            Vector2 joyR = new Vector2(hr, vr);
            bool attack = Input.GetButtonDown("Attack" + GetControllersuffix());
            bool attackUp = Input.GetButtonUp("Attack" + GetControllersuffix());
            bool attackPressed = Input.GetButton("Attack" + GetControllersuffix());
            bool defense = Input.GetButtonDown("Defense" + GetControllersuffix());
            bool defenseUp = Input.GetButtonUp("Defense" + GetControllersuffix());
            bool defensePressed = Input.GetButton("Defense" + GetControllersuffix());
            bool movement = Input.GetButtonDown("Movement" + GetControllersuffix());
            bool special = Input.GetButtonDown("Special" + GetControllersuffix());

            float modifier = Input.GetAxis("Modifier" + GetControllersuffix());
            bool modifierActive = modifier > 0.6f && modifier >= prevModifier;
            prevModifier = modifier;

            bool directionActive = false;

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
                m_Dir = m_Move;

                if (m_Move.magnitude == 0.0f && joyR.magnitude > 0.3f)
                {
                    m_Dir = vr * m_CamForward + hr * m_Cam.right;
                    directionActive = true;
                    //Debug.Log(m_Move.magnitude);
                }
            }

            VBGCharacterController.Action action = VBGCharacterController.Action.NONE;

            if(modifierActive)
            {
                if (m_aimingSpeAttack && attackUp)
                {
                    action = VBGCharacterController.Action.SPE_ATTACK;
                    m_aimingSpeAttack = false;
                }
                else if (attack)
                {
                    if(m_aimSpeAttack)
                    {
                        action = VBGCharacterController.Action.SPE_ATTACK_AIM;
                        m_aimingSpeAttack = true;
                    }
                    else
                    {
                        action = VBGCharacterController.Action.SPE_ATTACK;
                    }
                }
                if (m_aimingSpeDefense && defenseUp)
                {
                    action = VBGCharacterController.Action.SPE_DEFENSE;
                    m_aimingSpeDefense = false;
                }
                else if (special)
                {
                    action = VBGCharacterController.Action.SPE_SPECIAL;
                }
                else if (movement)
                {
                    action = VBGCharacterController.Action.SPE_MOVEMENT;
                }
                else if(defense)
                {
                    if (m_aimSpeDefense)
                    {
                        action = VBGCharacterController.Action.SPE_DEFENSE_AIM;
                        m_aimingSpeDefense = true;
                    }
                    else
                    {
                        action = VBGCharacterController.Action.SPE_DEFENSE;
                    }
                }
            } else
            {
                if (m_aimingAttack && attackUp)
                {
                    action = VBGCharacterController.Action.ATTACK;
                    m_aimingAttack = false;
                }
                else if (attack)
                {
                    if (m_aimAttack)
                    {
                        action = VBGCharacterController.Action.ATTACK_AIM;
                        m_aimingAttack = true;
                    }
                    else
                    {
                        action = VBGCharacterController.Action.ATTACK;
                    }
                }
                else if (special)
                {
                    action = VBGCharacterController.Action.SPECIAL;
                }
                else if (movement)
                {
                    action = VBGCharacterController.Action.MOVEMENT;
                }
                else if (defense)
                {
                    action = VBGCharacterController.Action.DEFENSE;
                }
            }

            if (m_aimingAttack && attackPressed && action == VBGCharacterController.Action.NONE)
            {
                action = VBGCharacterController.Action.ATTACK_AIM;
            }
            if (m_aimingAttack && !attackPressed && action == VBGCharacterController.Action.NONE)
            {
                action = VBGCharacterController.Action.ATTACK;
            }

            VBGCharacterController.Request request = new VBGCharacterController.Request
            {
                move = m_Move,
                direction = m_Dir,
                inputNorm = joy.magnitude,
                action = action,
                directionNorm = directionActive ? joyR.magnitude : 0.0f,
                modifier = modifierActive
            };

            // pass all parameters to the character control script
            m_Character.Move(request);
        }

        public void SetController(int _controllerID)
        {
            controllerID = _controllerID;
        }
    }
}