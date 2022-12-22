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
    public class PauseSettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI VolumeText;
        [SerializeField]
        private AudioMixer AudioMixer;
        [SerializeField]
        private TMP_Dropdown ResolutionDropdown;
        [SerializeField]
        private Toggle FullscreenToggle;

        private Resolution[] _screenResolutions;
        public void UpdateVolume(float value)
        {
            var val = RemapIntValue(Mathf.RoundToInt(value), -80, 0, 0, 100);
            VolumeText.text = $"{val}";
            AudioMixer.SetFloat("VolumeParam", value);
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

            FullscreenToggle.isOn = Screen.fullScreen;
            AudioMixer.GetFloat("VolumeParam", out var value);
            UpdateVolume(value);
        }
        public void ToggleFullscreen(bool value)
        {
            Screen.fullScreen = value;
        }

        private int RemapIntValue(int src, int srcFrom, int srcTo, int targetFrom, int targetTo)
        {
            return targetFrom + (src - srcFrom) * (targetTo - targetFrom) / (srcTo - srcFrom);
        }
    }
}