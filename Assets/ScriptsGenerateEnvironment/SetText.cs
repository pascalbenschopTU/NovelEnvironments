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
                    string text = "Walk through the gate to go to the next environment";
                    if (environmentConfiguration.InteractionConfig == ConfigType.Low)
                    {
                        text += "\n\nIn the environment you do not have to control the player (ignore the Controls once in the environment)";
                    }
                    if (environmentConfiguration.CameraTask)
                    {
                        text += "\n\nIn the environment you can take pictures"; 
                    }
                    if (environmentConfiguration.PickupTask)
                    {
                        text += "\n\nIn the environment you can pick up lanterns, in the corner one is placed as example";
                        PlaceLantern();
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

    private void PlaceLantern()
    {
        GameObject lantern = (GameObject) Resources.Load("Gathering/Prefabs/SM_Item_Lantern_01", typeof(GameObject));
        lantern.tag = "Gather";
        lantern.transform.localScale = new Vector3(4, 4, 4);
        lantern.layer = LayerMask.NameToLayer("Ground");
        GameObject player = GameObject.Find("Player");
        player.AddComponent<Gathering>();

        Instantiate(lantern, new Vector3(10f, 0.5f,-10f), Quaternion.identity);
    }
}
