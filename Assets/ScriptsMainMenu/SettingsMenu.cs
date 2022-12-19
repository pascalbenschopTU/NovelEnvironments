using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TextMeshProUGUI VolumeText;
    public AudioMixer AudioMixer;
    public TMP_InputField ModuloInput;
    public TMP_InputField TimeInput;
    public Toggle ModuloToggle;
    public TMP_Dropdown ResolutionDropdown;
    public Toggle FullscreenToggle;
    private Resolution[] _screenResolutions;
    
    public GameObject ButtonChooseFile;
    public GameObject ButtonChangeFile;
    public TextMeshProUGUI TextFilename;
    public void UpdateVolume(float value)
    {
        var val = RemapIntValue(Mathf.RoundToInt(value), -80, 0, 0, 100);
        VolumeText.text = $"{val}";
        PlayerPrefs.SetFloat("VolumeParam", val);

        Debug.Log($"Updated Volume to {value}");
    }

    public void UpdateResolution(int index)
    {
        var res = _screenResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen, res.refreshRate);
        Debug.Log($"Updated Resolution to {res.ToString()}");
    }
    
    public void ChooseExperimentFileButtonCallback()
    {
        var a = FileBrowser.ShowLoadDialog(paths =>
        {
            ChooseExperimentFile(paths[0]);
        }, () => {}, FileBrowser.PickMode.Files);
    }

    private void ChooseExperimentFile(string path)
    {
        ButtonChangeFile.SetActive(true);
        ButtonChooseFile.SetActive(false);
        TextFilename.gameObject.SetActive(true);
        List<string> parts;
        if (path.Contains("/"))
        {
            parts = path.Split("/").ToList();
        }else if (path.Contains("\\"))
        {
            parts = path.Split("\\").ToList();
        }
        else
        {
            return;
        }

        TextFilename.text = parts[^1];
        PlayerPrefs.SetString("SelectedFile", path);
    }

    public void UpdateGameTime(string time)
    {
        ExperimentMetaData.TimeInEnvironment = int.TryParse(time, out var outVal) ? outVal : 20;
        PlayerPrefs.SetInt("TimeSetting", ExperimentMetaData.TimeInEnvironment);
        Debug.Log($"Updated Time to {ExperimentMetaData.TimeInEnvironment}");
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
    
        if(!PlayerPrefs.HasKey("FullScreenSetting")) PlayerPrefs.SetInt("FullScreenSetting",Convert.ToInt32(true));
        if(!PlayerPrefs.HasKey("ModuloActiveSetting")) PlayerPrefs.SetInt("ModuloActiveSetting",Convert.ToInt32(true));
        if(!PlayerPrefs.HasKey("ModuloSetting")) PlayerPrefs.SetInt("ModuloSetting",10);
        if(!PlayerPrefs.HasKey("VolumeSetting")) PlayerPrefs.SetInt("VolumeSetting",100);
        if(!PlayerPrefs.HasKey("TimeSetting")) PlayerPrefs.SetInt("TimeSetting", 20);
        if (!PlayerPrefs.HasKey("SelectedFile"))
        {
            ButtonChooseFile.SetActive(true);
            ButtonChangeFile.SetActive(false);
            TextFilename.gameObject.SetActive(false);
            TextFilename.text = "";
        }else
        {
            ChooseExperimentFile(PlayerPrefs.GetString("SelectedFile"));
        }

        FullscreenToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("FullScreenSetting"));
        ModuloToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("ModuloActiveSetting"));
        ModuloInput.text = $"{PlayerPrefs.GetInt("ModuloSetting")}";
        TimeInput.text = $"{PlayerPrefs.GetInt("TimeSetting")}";
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
    }
    public void SaveModuloValue(string value)
    {
        if (int.TryParse(value, out var val))
        {
            PlayerPrefs.SetInt("ModuloSetting", val);
        }
    }

    public void SaveConfigs()
    {
        PlayerPrefs.Save();
    }
    
    private int RemapIntValue(int src, int srcFrom, int srcTo, int targetFrom, int targetTo)
    {
        return targetFrom + (src - srcFrom) * (targetTo - targetFrom) / (srcTo - srcFrom);
    }
}
