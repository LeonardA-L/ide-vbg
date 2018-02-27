using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class HUDHelper : MonoBehaviour
    {
        private Animator compass;
        private VBGCharacterController controller;
        private float timer = 0.0f;

        // Use this for initialization
        void Start()
        {
            controller = transform.parent.gameObject.GetComponent<VBGCharacterController>();
            Transform compassObject = transform.Find("Compass");
            if (compassObject != null)
            {
                compass = compassObject.gameObject.GetComponent<Animator>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(compass);
            if(compass != null)
            {
                UpdateCompass();
            }
        }

        private void UpdateCompass()
        {
            float dirNorm = controller.GetDirectionNorm();
            Debug.Log(dirNorm);
            if (dirNorm > 0.0f)
            {
                timer = GameManager.Constants.HUD_COMPASS_FADEOUT;
            }
            else
            {
                timer -= Time.deltaTime;
            }
            timer = Mathf.Clamp(timer, 0.0f, GameManager.Constants.HUD_COMPASS_FADEOUT);
            Debug.Log(timer);
            compass.SetBool("Active", timer > 0.0f);
        }
    }
}