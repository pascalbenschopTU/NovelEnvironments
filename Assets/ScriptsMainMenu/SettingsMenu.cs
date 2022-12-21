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
    public GameObject ExperimentSelection;
    public TextMeshProUGUI TextFilename;
    public TMP_Dropdown experimentSelectionDropdown;
    
    public Dictionary<int, List<EnvironmentConfiguration>> EnvironmentConfigurations { get; private set; }
    public int ExperimentId { get; private set; }
    public void UpdateVolume(float value)
    {
        var val = RemapIntValue(Mathf.RoundToInt(value), -80, 0, 0, 100);
        VolumeText.text = $"{val}";
        PlayerPrefs.SetFloat("VolumeParam", val);
    }

    public void UpdateResolution(int index)
    {
        var res = _screenResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen, res.refreshRate);
    }

    public void UpdateExperiment(int index)
    {
        ExperimentId = index;
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
        LoadExperimentsFromFile();
    }
    
    public void LoadExperimentsFromFile()
    {
        experimentSelectionDropdown.ClearOptions();
        if (PlayerPrefs.HasKey("SelectedFile"))
        {
           EnvironmentConfigurations = CsvUtils.EnvironmentConfigsFromCsv(PlayerPrefs.GetString("SelectedFile"));
           Debug.Log($"Loaded data from: {PlayerPrefs.GetString("SelectedFile")}");
           experimentSelectionDropdown.AddOptions(EnvironmentConfigurations.Keys.ToList().ConvertAll(k => k.ToString()));
           ExperimentId = 0;
           experimentSelectionDropdown.value = ExperimentId;
           ExperimentSelection.SetActive(!ModuloToggle.isOn && EnvironmentConfigurations != null && EnvironmentConfigurations.Count > 0);
        }
    }
    public void UpdateGameTime(string time)
    {
        ExperimentMetaData.TimeInEnvironment = int.TryParse(time, out var outVal) ? outVal : 20;
        PlayerPrefs.SetInt("TimeSetting", ExperimentMetaData.TimeInEnvironment);
        Debug.Log($"Updated Time to {ExperimentMetaData.TimeInEnvironment}");
    }

    public void LoadSettingsMenu()
    {
        ExperimentSelection.SetActive(!ModuloToggle.isOn && EnvironmentConfigurations != null && EnvironmentConfigurations.Count > 0);
    }

    // Start is called before the first frame update
    void Awake()
    {
        _screenResolutions = Screen.resolutions;
        var currentRes = Screen.currentResolution;
        ResolutionDropdown.ClearOptions();
        experimentSelectionDropdown.ClearOptions();
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
    
        PlayerPrefs.DeleteKey("SelectedFile");
        
        if(!PlayerPrefs.HasKey("FullScreenSetting")) PlayerPrefs.SetInt("FullScreenSetting",1);
        if(!PlayerPrefs.HasKey("ModuloActiveSetting")) PlayerPrefs.SetInt("ModuloActiveSetting",1);
        if(!PlayerPrefs.HasKey("ModuloSetting")) PlayerPrefs.SetInt("ModuloSetting",10);
        if(!PlayerPrefs.HasKey("VolumeSetting")) PlayerPrefs.SetInt("VolumeSetting",100);
        if(!PlayerPrefs.HasKey("TimeSetting")) PlayerPrefs.SetInt("TimeSetting", 20);
        
        FullscreenToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("FullScreenSetting"));
        ModuloToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("ModuloActiveSetting"));
        ModuloInput.text = $"{PlayerPrefs.GetInt("ModuloSetting")}";
        TimeInput.text = $"{PlayerPrefs.GetInt("TimeSetting")}";
        
        if (!PlayerPrefs.HasKey("SelectedFile"))
        {
            ButtonChooseFile.SetActive(true);
            ButtonChangeFile.SetActive(false);
            TextFilename.gameObject.SetActive(false);
            TextFilename.text = "";
            ExperimentSelection.SetActive(false);
            EnvironmentConfigurations = new Dictionary<int, List<EnvironmentConfiguration>>();
        }
        else
        {
            ChooseExperimentFile(PlayerPrefs.GetString("SelectedFile"));
        }
        
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
        ExperimentSelection.SetActive(!ModuloToggle.isOn && EnvironmentConfigurations != null && EnvironmentConfigurations.Count > 0);
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