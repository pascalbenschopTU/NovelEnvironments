using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TextMeshProUGUI VolumeText;
    public AudioMixer AudioMixer;
    public TMP_InputField ModuloInput;
    public Toggle ModuloToggle;
    public TMP_Dropdown ResolutionDropdown;
    public Toggle FullscreenToggle;
    private Resolution[] _screenResolutions;
    public void UpdateVolume(float value)
    {
        
        VolumeText.text = string.Format("{0}", RemapIntValue(Mathf.RoundToInt(value), -80, 0 , 0 , 100));
        AudioMixer.SetFloat("VolumeParam", value);
        Debug.Log($"Updated Volume to {value}");
    }

    public void UpdateResolution(int index)
    {
        var res = _screenResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen, res.refreshRate);
        Debug.Log($"Updated Resolution to {res.ToString()}");
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        _screenResolutions = Screen.resolutions;
        var currentRes = Screen.currentResolution;
        ResolutionDropdown.ClearOptions();
        var resolutions = new List<string>();
        var index = 0;
        for(var i = 0; i < _screenResolutions.Length; i++)
        {
            var s = _screenResolutions[i].ToString();
            resolutions.Add(s);
            if (s == currentRes.ToString())
            {
                index = i;
            }
        }
        ResolutionDropdown.AddOptions(resolutions);
        ResolutionDropdown.value = index;

        FullscreenToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("FullScreenSetting"));
        ModuloToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("ModuloActiveSetting"));
        ModuloInput.text = $"{PlayerPrefs.GetInt("ModuloSetting")}";
        UpdateVolume(PlayerPrefs.GetFloat("VolumeSetting"));
    }
    public void ToggleFullscreen(bool value)
    {
        Screen.fullScreen = value;
        Debug.Log($"Set fullscreen to {value}");
        PlayerPrefs.SetInt("FullScreenSetting", Convert.ToInt32(value));
    }
    public void ToggleModuloActive(bool active)
    {
        PlayerPrefs.SetInt("ModuloActiveSetting", Convert.ToInt32(active));
        PlayerPrefs.Save();
    }
    public void SetModuloValue(int value)
    {
        PlayerPrefs.SetInt("ModuloSetting", value);
        PlayerPrefs.Save();
    }
    
    private int RemapIntValue(int src, int srcFrom, int srcTo, int targetFrom, int targetTo)
    {
        return targetFrom + (src - srcFrom) * (targetTo - targetFrom) / (srcTo - srcFrom);
    }
}
