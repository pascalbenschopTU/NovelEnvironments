using UnityEngine;
using TMPro;

public class SetText : MonoBehaviour
{

    public TMP_Text mText;
    
    // Set text for instructions in Default Environment
    void Start()
    {
        if (ExperimentMetaData.Index < ExperimentMetaData.Environments.Count)
        {
            EnvironmentConfiguration environmentConfiguration = ExperimentMetaData.Environments[ExperimentMetaData.Index];
            switch (gameObject.name)
            {
                case "Text (TMP) Instructions":
                    string text = "";
                    if (PlayerPrefs.HasKey("InstructionSetting"))
                    {
                        text += PlayerPrefs.GetString("InstructionSetting");
                    }
                    else
                    {
                        text += "Walk through the gate to go to the next environment";
                    }
                    if (environmentConfiguration.CameraTask)
                    {
                        text += "\n\nIn the environment you can take pictures"; 
                    }
                    if (environmentConfiguration.PickupTask)
                    {
                        text += "\n\nIn the environment you can pick up lanterns";
                    }
                    mText.text = text;
                    break;
                case "Text (TMP) ControlsCamera":
                    if (environmentConfiguration.CameraTask)
                    {
                        mText.enabled = true;
                    }
                    else
                    {
                        mText.enabled = false;
                    }
                    break;
                case "Text (TMP) ControlsCollecting":
                    if (environmentConfiguration.PickupTask)
                    {
                        mText.enabled = true;
                    }
                    else
                    {
                        mText.enabled = false;
                    }
                    break;
                default:
                    break;
            }
        }
        // Last time in the default scene
        else
        {
            if (gameObject.name == "Text (TMP) Instructions")
            {
                mText.text = "Walk through the gate to finish the game";
            }
            else
            {
                mText.enabled = false;
            }
        }
    }
}
