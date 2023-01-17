using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class EnableTextCamera : MonoBehaviour
{
    public TMP_Text mText;

    void Start()
    {
        EnvironmentConfiguration environmentConfiguration = ExperimentMetaData.Environments[ExperimentMetaData.Index];
        if(environmentConfiguration.CameraTask) {
            mText.enabled = true;
        }
        else {
            mText.enabled = false;
        }      
    }
}
