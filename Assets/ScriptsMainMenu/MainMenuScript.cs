using UnityEngine;

namespace ScriptsMainMenu
{
    public class MainMenuScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject SettingsMenu;
        [SerializeField]
        private GameObject ExperimentCreator;
        [SerializeField]
        private GameObject StartMenu;        
        [SerializeField]
        private GameObject MainMenu;
        [SerializeField]
        private GameObject FinishScreen;
        void Awake()
        {
            SettingsMenu.SetActive(true);        
            ExperimentCreator.SetActive(true);        
            StartMenu.SetActive(true);        
            ExperimentCreator.SetActive(false);        
            SettingsMenu.SetActive(false);        
            StartMenu.SetActive(false);        
            FinishScreen.SetActive(false);
            if (ExperimentMetaData.ExperimentFinished)
            {
                MainMenu.SetActive(false);
                FinishScreen.SetActive(true);
                var script = FinishScreen.GetComponent<EndScreen>();
                script.ShowEndScreen();
            }
            
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
