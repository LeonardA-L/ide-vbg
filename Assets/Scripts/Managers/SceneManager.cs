using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg {
    public class SceneManager : MonoBehaviour {

        public enum Scene
        {
            MENU,
            DEMO,
        }

        protected static SceneManager m_instance;
        public static SceneManager Instance
        {
            get
            {
                return m_instance;
            }
        }

        void Start()
        {
            m_instance = this;
        }


        IEnumerator LoadSceneAsync(Scene _scene)
        {
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)_scene);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        public void LaunchScene(Scene _scene)
        {
            StartCoroutine(LoadSceneAsync(_scene));
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}