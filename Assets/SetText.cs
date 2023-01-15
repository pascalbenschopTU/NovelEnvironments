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
        if (PlayerPrefs.HasKey("InstructionSetting")) {
            mText.text = "\n" + PlayerPrefs.GetString("InstructionSetting");
            // mText.text = "BBB";
        }
        else {
           mText.text = "Walk through the gate to go to the next environment"; 
        }
    }
}
