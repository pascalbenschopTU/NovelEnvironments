using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnableTextGathering : MonoBehaviour
{
    public TMP_Text mText;

    void Start()
    {
        EnvironmentConfiguration environmentConfiguration = ExperimentMetaData.Environments[ExperimentMetaData.Index];
        if(environmentConfiguration.PickupTask) {
            mText.enabled = true;
        }
        else {
            mText.enabled = false;
        }        
    }
}
