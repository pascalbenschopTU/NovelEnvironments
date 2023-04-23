using UnityEngine;
using TMPro;

public class SetText : MonoBehaviour
{

    public TMP_Text mText;
    
    // Set text for instructions in Default Environment
    void OnEnable()
    {
        if (ExperimentMetaData.Index < ExperimentMetaData.Environments.Count)
        {
            EnvironmentConfiguration environmentConfiguration = ExperimentMetaData.Environments[ExperimentMetaData.Index];
            switch (gameObject.name)
            {
                case "Text (TMP) Instructions":
                    string text = "Walk through the gate to go to the next environment";
                    if (environmentConfiguration.InteractionConfig == ConfigType.Low)
                    {
                        text += "\n\nIn the environment you do not have to control the player (ignore the Controls once in the environment)";
                    }
                    if (environmentConfiguration.MapConfig == ConfigType.High)
                    {
                        text += "\n\nYou can view a map of the environment on your left";
                    }
                    if (environmentConfiguration.CameraTask)
                    {
                        text += "\n\nIn the environment you can take pictures"; 
                    }
                    if (environmentConfiguration.PickupTask)
                    {
                        text += "\n\nIn the environment you can pick up lanterns, in the corner one is placed as example";
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
