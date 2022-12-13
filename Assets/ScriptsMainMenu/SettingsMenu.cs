using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public TextMeshProUGUI VolumeText;
    public AudioMixer AudioMixer;
    public TMP_InputField ModuloInput;
    public TMP_Dropdown ResolutionDropdown;
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
    
    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
        Debug.Log($"Set fullscreen to {value}");
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

        ModuloInput.text = "10";
        AudioMixer.GetFloat("VolumeParam", out var outVal);
        UpdateVolume(outVal);
    }

    private int RemapIntValue(int src, int srcFrom, int srcTo, int targetFrom, int targetTo)
    {
        return targetFrom + (src - srcFrom) * (targetTo - targetFrom) / (srcTo - srcFrom);
    }
}
