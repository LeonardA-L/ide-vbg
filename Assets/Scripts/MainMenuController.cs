using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace vbg
{
    public class MainMenuController : MonoBehaviour
    {
        public Animator cameraAnimator;

        public List<Button> heads;

        // Use this for initialization
        void Start()
        {

        }

        public void MoveTo(string scene)
        {
            int buttonId = 0;
            switch (scene)
            {
                case "Launch":
                    buttonId = 1;
                    break;
                case "Credits":
                    buttonId = 2;
                    break;
                case "Back":
                default:
                    buttonId = 0;
                    break;
            }
            heads[buttonId].Select();
            cameraAnimator.SetTrigger(scene);
        }

        public void StartDemo()
        {
            SceneManager.Instance.LaunchScene(SceneManager.Scene.DEMO);
        }
    }
}
