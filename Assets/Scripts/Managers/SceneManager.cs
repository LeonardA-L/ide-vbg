using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vbg {
    public class SceneManager : MonoBehaviour {

        public enum Scene
        {
            MENU,
            DEMO,
            ARENA,
            UB85,
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
            SoundManager.Instance.PostEvent("Stop_All_Scene", gameObject);
            StartCoroutine(LoadSceneAsync(_scene));
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void QuitToMenu()
        {
            LaunchScene(Scene.MENU);
        }

        public void LaunchGame()
        {
            if (!enabled)
                return;
            SceneManager.Instance.LaunchScene(SceneManager.Scene.DEMO);
            enabled = false;
        }
    }
}