using UnityEngine;

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
            bool jump = Input.GetButtonDown("Jump" + GetControllersuffix());

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }

            // pass all parameters to the character control script
            m_Character.Move(m_Move, joy.magnitude, jump);
            jump = false;
        }

        public void SetController(int _controllerID)
        {
            controllerID = _controllerID;
        }
    }
}