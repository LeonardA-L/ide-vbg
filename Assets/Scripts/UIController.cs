using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace vbg
{
    public class UIController : MonoBehaviour
    {
        public GameObject pauseMenu;
        public Button button;
       

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetPauseMenu(bool _active)
        {
            pauseMenu.active = _active;
            button.Select();
        }
    }
}