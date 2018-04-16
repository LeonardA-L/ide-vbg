using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg
{
    public class UIController : MonoBehaviour
    {
        public GameObject pauseMenu;

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
        }
    }
}