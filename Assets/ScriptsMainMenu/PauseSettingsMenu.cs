using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace ScriptsMainMenu
{
    public class PauseSettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI VolumeText;
        [SerializeField]
        private TextMeshProUGUI MouseSensitivityText;
        [SerializeField] 
        private Slider VolumeSlider;
        [SerializeField] 
        private Slider MouseSensitivitySlider;
        [SerializeField]
        private AudioMixer AudioMixer;
        [SerializeField]
        private TMP_Dropdown ResolutionDropdown;
        [SerializeField]
        private Toggle FullscreenToggle;

        private Resolution[] _screenResolutions;
        public void UpdateVolume(float value)
        {
            var valueInt = Mathf.RoundToInt(value);
            AudioMixer.SetFloat("VolumeParam", value);
            PlayerPrefs.SetInt("VolumeSetting", valueInt);
            var val = SettingsMenu.RemapIntValue(valueInt, SettingsMenu.MinVolume, SettingsMenu.MaxVolume, 0, 100);
            VolumeText.text = $"{val}";
        }
        
        public void UpdateMouseSensitivity(float value)
        {
            var valueInt = Mathf.RoundToInt(value);
            PlayerPrefs.SetInt("MouseSensitivitySetting", valueInt);
            var val = SettingsMenu.RemapIntValue(valueInt, SettingsMenu.MinMouseSensitivity, SettingsMenu.MaxMouseSensitivity, 0, 100);
            MouseLook.MouseSensitivity = valueInt;
            MouseSensitivityText.text = $"{val}";
        }

        public void UpdateResolution(int index)
        {
            var res = _screenResolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen, res.refreshRate);
        }

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
            AudioMixer.GetFloat("VolumeParam", out var value);
            UpdateMouseSensitivity(PlayerPrefs.GetInt("MouseSensitivitySetting"));
            MouseSensitivitySlider.value = PlayerPrefs.GetInt("MouseSensitivitySetting");
            UpdateVolume(PlayerPrefs.GetInt("VolumeSetting"));
            VolumeSlider.value = PlayerPrefs.GetInt("VolumeSetting");
        }
        public void ToggleFullscreen(bool value)
        {
            PlayerPrefs.SetInt("FullScreenSetting", Convert.ToInt32(value));
            Screen.fullScreen = value;
        }
    }
}