using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject SettingsMenu;
    public GameObject ExperimentCreator;
    public GameObject StartMenu;
    // Start is called before the first frame update
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
