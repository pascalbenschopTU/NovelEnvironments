using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace ScriptsMainMenu
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI VolumeText;
        [SerializeField]
        private TextMeshProUGUI MouseSensitivityText;

        [SerializeField] private Slider VolumeSlider;
        [SerializeField] private Slider MouseSensitivitySlider;
        [SerializeField]
        private AudioMixer AudioMixer;
        [SerializeField]
        private TMP_InputField ModuloInput;
        [SerializeField]
        private TMP_InputField TimeInput;
        [SerializeField]
        private Toggle ModuloToggle;
        [SerializeField]
        private TMP_Dropdown ResolutionDropdown;
        [SerializeField]
        private Toggle FullscreenToggle;
    
        [SerializeField]
        private GameObject ButtonChooseFile;
        [SerializeField]
        private GameObject ButtonChangeFile;
        [SerializeField]
        private GameObject ExperimentSelection;
        [SerializeField]
        private TextMeshProUGUI TextFilename;
        [SerializeField]
        private TMP_Dropdown experimentSelectionDropdown;

        public static int MinVolume = -80;
        public static int MaxVolume = 0;
        public static int MinMouseSensitivity = 50;
        public static int MaxMouseSensitivity = 1000;

        private Resolution[] _screenResolutions;
        public Dictionary<int, List<EnvironmentConfiguration>> EnvironmentConfigurations { get; private set; }
        public int ExperimentId { get; private set; }
        public void UpdateVolume(float value)
        {
            var valueInt = Mathf.RoundToInt(value);
            AudioMixer.SetFloat("VolumeParam", value);
            PlayerPrefs.SetInt("VolumeSetting", valueInt);
            var val = RemapIntValue(valueInt, MinVolume, MaxVolume, 0, 100);
            VolumeText.text = $"{val}";
        }

        public void UpdateMouseSensitivity(float value)
        {
            var valueInt = Mathf.RoundToInt(value);
            PlayerPrefs.SetInt("MouseSensitivitySetting", valueInt);
            var val = RemapIntValue(valueInt, MinMouseSensitivity, MaxMouseSensitivity, 0, 100);
            MouseSensitivityText.text = $"{val}";
        }

        public void UpdateResolution(int index)
        {
            var res = _screenResolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen, res.refreshRate);
        }

        public void UpdateExperiment(int index)
        {
            ExperimentId = index;
            PlayerPrefs.SetInt("LastExperimentId", ExperimentId);
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
                ExperimentId = PlayerPrefs.HasKey("LastExperimentId") && EnvironmentConfigurations.Keys.Contains(PlayerPrefs.GetInt("LastExperimentId"))? PlayerPrefs.GetInt("LastExperimentId"): 0;
                PlayerPrefs.SetInt("LastExperimentId", ExperimentId);
                experimentSelectionDropdown.value = ExperimentId;
                ExperimentSelection.SetActive(!ModuloToggle.isOn && EnvironmentConfigurations != null && EnvironmentConfigurations.Count > 0);
            }
        }
        public void UpdateGameTime(string time)
        {
            ExperimentMetaData.TimeInEnvironment = int.TryParse(time, out var outVal) ? outVal : 20;
            PlayerPrefs.SetInt("TimeSetting", ExperimentMetaData.TimeInEnvironment);
        }

        public void LoadSettingsMenu()
        {
            ExperimentSelection.SetActive(!ModuloToggle.isOn && EnvironmentConfigurations != null && EnvironmentConfigurations.Count > 0);
        }

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
            
            if(!PlayerPrefs.HasKey("FullScreenSetting")) PlayerPrefs.SetInt("FullScreenSetting",1);
            if(!PlayerPrefs.HasKey("ModuloActiveSetting")) PlayerPrefs.SetInt("ModuloActiveSetting",1);
            if(!PlayerPrefs.HasKey("ModuloSetting")) PlayerPrefs.SetInt("ModuloSetting",10);
            if (!PlayerPrefs.HasKey("VolumeSetting")) PlayerPrefs.SetInt("VolumeSetting", -40);
            if(!PlayerPrefs.HasKey("TimeSetting")) PlayerPrefs.SetInt("TimeSetting", 20);
            if (!PlayerPrefs.HasKey("MouseSensitivitySetting")) PlayerPrefs.SetInt("MouseSensitivitySetting", 300);

            ToggleFullscreen(Convert.ToBoolean(PlayerPrefs.GetInt("FullScreenSetting")));
            FullscreenToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("FullScreenSetting"));
            ToggleModuloActive(Convert.ToBoolean(PlayerPrefs.GetInt("ModuloActiveSetting")));
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

            MouseSensitivitySlider.minValue = MinMouseSensitivity;
            MouseSensitivitySlider.maxValue = MaxMouseSensitivity;
            VolumeSlider.minValue = MinVolume;
            VolumeSlider.maxValue = MaxVolume;
            
            UpdateMouseSensitivity(PlayerPrefs.GetInt("MouseSensitivitySetting"));
            MouseSensitivitySlider.value = PlayerPrefs.GetInt("MouseSensitivitySetting");
            UpdateVolume(PlayerPrefs.GetInt("VolumeSetting"));
            VolumeSlider.value = PlayerPrefs.GetInt("VolumeSetting");
        }
        public void ToggleFullscreen(bool value)
        {
            Screen.fullScreen = value;
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
    
        public static int RemapIntValue(int src, int srcFrom, int srcTo, int targetFrom, int targetTo)
        {
            return targetFrom + (src - srcFrom) * (targetTo - targetFrom) / (srcTo - srcFrom);
        }
    }
}