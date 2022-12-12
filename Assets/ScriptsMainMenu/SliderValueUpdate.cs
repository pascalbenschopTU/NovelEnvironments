using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueUpdate : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Slider slider;
    // Start is called before the first frame update
    void OnEnable(){
        slider.onValueChanged.AddListener(ChangeValue);
        ChangeValue(slider.value);
    }

    void OnDisable(){
        slider.onValueChanged.RemoveAllListeners();
    }

    void ChangeValue(float value){
        text.text = value.ToString() + "%";
    }
}
