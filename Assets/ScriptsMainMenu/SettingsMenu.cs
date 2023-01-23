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
        [SerializeField] private TextMeshProUGUI VolumeText;
        [SerializeField] private TextMeshProUGUI MouseSensitivityText;

        [SerializeField] private Slider VolumeSlider;
        [SerializeField] private Slider MouseSensitivitySlider;
        [SerializeField] private AudioMixer AudioMixer;
        [SerializeField] private TMP_InputField TimeInput;
        [SerializeField] private TMP_Dropdown ResolutionDropdown;
        [SerializeField] private Toggle FullscreenToggle;
        [SerializeField] private TMP_InputField ModuloInput;
        [SerializeField] private Toggle ModuloToggle;
        [SerializeField] private TMP_InputField SeedInputField;
        [SerializeField] private Toggle SeedToggle;
        [SerializeField] private Toggle EndScreenToggle;
    
        [SerializeField] private GameObject ButtonChooseFile;
        [SerializeField] private GameObject ButtonChangeFile;
        [SerializeField] private GameObject ExperimentSelection;
        [SerializeField] private TextMeshProUGUI TextFilename;
        [SerializeField] private TMP_Dropdown experimentSelectionDropdown;

        
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
        public void UpdateModuloValue(string value)
        {
            if (int.TryParse(value, out var val))
            {
                PlayerPrefs.SetInt("ModuloSetting", val);
            }
        }
        public void UpdateGameTime(string time)
        {
            ExperimentMetaData.TimeInEnvironment = int.TryParse(time, out var outVal) ? outVal : 180;
            PlayerPrefs.SetInt("TimeSetting", ExperimentMetaData.TimeInEnvironment);
        }

        public void UpdateSeed(string seed)
        {
            if (int.TryParse(seed, out var val))
            {
                PlayerPrefs.SetInt("SeedSetting", val);
            }
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

        private void ClearExperimentFile()
        {
            ButtonChangeFile.SetActive(false);
            ButtonChooseFile.SetActive(true);
            TextFilename.gameObject.SetActive(false);
            ExperimentSelection.SetActive(false);
        }
        
        public void LoadExperimentsFromFile()
        {
            experimentSelectionDropdown.ClearOptions();
            if (PlayerPrefs.HasKey("SelectedFile"))
            {
                EnvironmentConfigurations = CsvUtils.LoadEnvironmentConfigsFromCsv(PlayerPrefs.GetString("SelectedFile"));
                if (EnvironmentConfigurations.Count == 0)
                {
                    ClearExperimentFile();
                    Debug.Log($"Loading data from: {PlayerPrefs.GetString("SelectedFile")} failed!");
                    return;
                }
                Debug.Log($"Loaded data from: {PlayerPrefs.GetString("SelectedFile")}");
                experimentSelectionDropdown.AddOptions(EnvironmentConfigurations.Keys.ToList().ConvertAll(k => k.ToString()));
                ExperimentId = PlayerPrefs.HasKey("LastExperimentId") && EnvironmentConfigurations.Keys.Contains(PlayerPrefs.GetInt("LastExperimentId"))? PlayerPrefs.GetInt("LastExperimentId"): 0;
                PlayerPrefs.SetInt("LastExperimentId", ExperimentId);
                experimentSelectionDropdown.value = ExperimentId;
                ExperimentSelection.SetActive(!ModuloToggle.isOn && EnvironmentConfigurations != null && EnvironmentConfigurations.Count > 0);
            }
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
            if(!PlayerPrefs.HasKey("EndScreenActiveSetting")) PlayerPrefs.SetInt("EndScreenActiveSetting",1);
            if(!PlayerPrefs.HasKey("ModuloSetting")) PlayerPrefs.SetInt("ModuloSetting",10);
            if(!PlayerPrefs.HasKey("SeedActiveSetting")) PlayerPrefs.SetInt("SeedActiveSetting",0);
            if(!PlayerPrefs.HasKey("SeedSetting")) PlayerPrefs.SetInt("SeedSetting",1);
            if(!PlayerPrefs.HasKey("VolumeSetting")) PlayerPrefs.SetInt("VolumeSetting", -40);
            if(!PlayerPrefs.HasKey("TimeSetting")) PlayerPrefs.SetInt("TimeSetting", 20);
            if(!PlayerPrefs.HasKey("MouseSensitivitySetting")) PlayerPrefs.SetInt("MouseSensitivitySetting", 300);

            ToggleFullscreen(Convert.ToBoolean(PlayerPrefs.GetInt("FullScreenSetting")));
            FullscreenToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("FullScreenSetting"));
            ToggleModuloActive(Convert.ToBoolean(PlayerPrefs.GetInt("ModuloActiveSetting")));
            ModuloToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("ModuloActiveSetting"));
            ToggleSeedActive(Convert.ToBoolean(PlayerPrefs.GetInt("SeedActiveSetting")));
            SeedToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("SeedActiveSetting"));
            EndScreenToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("EndScreenActiveSetting"));
            
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
        public void ToggleShowEndScreenData(bool active)
        {
            PlayerPrefs.SetInt("EndScreenActiveSetting", Convert.ToInt32(active));
        }
        public void ToggleModuloActive(bool active)
        {
            PlayerPrefs.SetInt("ModuloActiveSetting", Convert.ToInt32(active));
            ExperimentSelection.SetActive(!ModuloToggle.isOn && EnvironmentConfigurations != null && EnvironmentConfigurations.Count > 0);
            if (active)
            {
                ModuloInput.gameObject.SetActive(true);
                ModuloInput.text = $"{PlayerPrefs.GetInt("ModuloSetting")}";
            }
            else
            {
                ModuloInput.gameObject.SetActive(false);
            }
        }

        public void ToggleSeedActive(bool active)
        {
            PlayerPrefs.SetInt("SeedActiveSetting", Convert.ToInt32(active));
            if (active)
            {
                SeedInputField.gameObject.SetActive(true);
                SeedInputField.text = $"{PlayerPrefs.GetInt("SeedSetting")}";
            }
            else
            {
                SeedInputField.gameObject.SetActive(false);
            }
        }

        public void SaveInstructionValue(string value)
        {
            PlayerPrefs.SetString("InstructionSetting", value);
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