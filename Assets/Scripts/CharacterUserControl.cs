﻿using UnityEngine;

namespace vbg
{
    [RequireComponent(typeof (VBGCharacterController))]
    public class CharacterUserControl : MonoBehaviour
    {
        private int controllerID = -1;

        private VBGCharacterController m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;

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
            Vector2 joy = new Vector2(h, v);
            bool attack = Input.GetButtonDown("Attack" + GetControllersuffix());
            bool defense = Input.GetButtonDown("Defense" + GetControllersuffix());
            bool movement = Input.GetButtonDown("Movement" + GetControllersuffix());
            bool special = Input.GetButtonDown("Special" + GetControllersuffix());

            float modifier = Input.GetAxis("Modifier" + GetControllersuffix());
            bool modifierActive = modifier > 0.6f && modifier >= prevModifier;
            prevModifier = modifier;

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }

            VBGCharacterController.Action action = VBGCharacterController.Action.NONE;

            if(modifierActive)
            {
                if(attack)
                {
                    action = VBGCharacterController.Action.SPE_ATTACK;
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
                    action = VBGCharacterController.Action.SPE_DEFENSE;
                }
            } else
            {
                if (attack)
                {
                    action = VBGCharacterController.Action.ATTACK;
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

            VBGCharacterController.Request request = new VBGCharacterController.Request
            {
                move = m_Move,
                inputNorm = joy.magnitude,
                action = action
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