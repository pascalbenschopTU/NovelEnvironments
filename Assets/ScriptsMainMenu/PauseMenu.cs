using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScriptsMainMenu
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] 
        private GameObject pauseMenu;        [SerializeField] 
        private GameObject pauseMenuSettings;
        private bool _paused;
    
        public void Pause()
        {
            Debug.Log("PAUSE");
            pauseMenu.SetActive(true);
            pauseMenuSettings.SetActive(false);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;

            _paused = true;
        }

        public void Resume()
        {
            Debug.Log("RESUME");
            pauseMenu.SetActive(false);
            pauseMenuSettings.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;

            _paused = false;
        }

        public void QuitExperiment()
        {
            Resume();
            Cursor.lockState = CursorLockMode.None;
            ExperimentMetaData.ExperimentFinished = true;
            SceneManager.LoadScene(0);
        }

        private void Awake()
        {
            Resume();
        }

        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("Button down!!");
                if (!_paused)
                {
                    Pause();
                }
                else
                {
                    Resume();
                }
            }
        }
    }
}
