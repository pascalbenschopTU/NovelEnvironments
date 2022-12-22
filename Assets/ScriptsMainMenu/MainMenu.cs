using UnityEngine;

namespace ScriptsMainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject SettingsMenu;
        [SerializeField]
        private GameObject ExperimentCreator;
        [SerializeField]
        private GameObject StartMenu;
        void Awake()
        {
            SettingsMenu.SetActive(true);        
            ExperimentCreator.SetActive(true);        
            StartMenu.SetActive(true);        
            ExperimentCreator.SetActive(false);        
            SettingsMenu.SetActive(false);        
            StartMenu.SetActive(false);        
        }

    }
}
