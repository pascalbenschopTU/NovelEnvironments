using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetText : MonoBehaviour
{

    public TMP_Text mText;
    
    // Start is called before the first frame update
    void Start()
    {
        EnvironmentConfiguration environmentConfiguration = ExperimentMetaData.Environments[ExperimentMetaData.Index];
        switch(gameObject.name) 
        {
            case "Text (TMP) Instruction":
                if (PlayerPrefs.HasKey("InstructionSetting")) {
                    mText.text = "\n" + PlayerPrefs.GetString("InstructionSetting");
                }
                else {
                mText.text = "Walk through the gate to go to the next environment"; 
                }
                break;
            case "Text (TMP) ControlsCamera":
                if(environmentConfiguration.CameraTask) {
                    mText.enabled = true;
                }
                else {
                    mText.enabled = false;
                }      
                break;
            case "Text (TMP) ControlsCollecting":
                if(environmentConfiguration.PickupTask) {
                    mText.enabled = true;
                }
                else {
                    mText.enabled = false;
                }   
                break;
            default:
                break;
        }
    }
}
