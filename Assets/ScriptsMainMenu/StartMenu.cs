using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScriptsMainMenu
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField InputParticipantNumber;
        [SerializeField]
        private TextMeshProUGUI ErrorText;
        [SerializeField]
        private SettingsMenu SettingsMenu;

        [SerializeField]
        private float ErrorTextTimeout;
        private int _participantNumber;
        private int _experimentId;
        private bool _fileSelected;
        private int _moduloValue;
        private bool _moduloActive;
        private int _vanishingRate;
        private float _errorEndTime;
        private bool _errorOccured;
        private Dictionary<int, List<EnvironmentConfiguration>> _environmentConfigurations;
    
        private void Awake()
        {
            InputParticipantNumber.text = string.Empty;
            ErrorText.text = "";
            _participantNumber = -1;
            _experimentId = -1;
            _fileSelected = false;
            _moduloValue = PlayerPrefs.HasKey("ModuloSetting") ? PlayerPrefs.GetInt("ModuloSetting"): 10;
            _moduloActive = PlayerPrefs.HasKey("ModuloActiveSetting") ? Convert.ToBoolean(PlayerPrefs.GetInt("ModuloActiveSetting")): true;
            _errorOccured = false;
            _errorEndTime = 0.0f;
            _vanishingRate = 10;
        }

        private void ShowErrorMessage(string msg)
        {
            ErrorText.text = msg;
            _errorEndTime = Time.time + ErrorTextTimeout;
            _errorOccured = true;
        }

        public void ChangeParticipantNumber(string input)
        {
            var output = int.TryParse(input, out var outVal) ? outVal: -1;
            _participantNumber = output;
            if (_participantNumber >= 0)
            {
                if (_moduloActive)
                {
                    _experimentId = _participantNumber % _moduloValue;
                    if (_fileSelected && _environmentConfigurations.Keys.ToList().Count <= _experimentId)
                    {
                        _experimentId = _environmentConfigurations.Keys.ToList().Count - 1;
                    }
                }
            }
        }

        public void LoadStartMenu()
        {
            _moduloActive = Convert.ToBoolean(PlayerPrefs.GetInt("ModuloActiveSetting"));
            _moduloValue = PlayerPrefs.HasKey("ModuloSetting") ? PlayerPrefs.GetInt("ModuloSetting"): 10;
            _environmentConfigurations = SettingsMenu.EnvironmentConfigurations;
            _fileSelected = _environmentConfigurations != null && _environmentConfigurations.Keys.Count > 0;
            if (!_fileSelected)
            {
                ShowErrorMessage("Check experiment file in settings menu!");
            }
            if (!_moduloActive)
            {
                _experimentId = SettingsMenu.ExperimentId;
            }
            else
            {
                _experimentId = _participantNumber % _moduloValue;
                if (_environmentConfigurations != null && _fileSelected && _environmentConfigurations.Keys.ToList().Count <= _experimentId)
                {
                    _experimentId = _environmentConfigurations.Keys.ToList().Count - 1;
                }
            }
        }
    
        public void StartGame()
        {
            if (_experimentId >= 0 && _fileSelected && _participantNumber >= 0)
            {
                // start game
                var list = _environmentConfigurations[_experimentId];
            
                //TODO generate seed or get it from somewhere
                ExperimentMetaData.Seed = 100;
                ExperimentMetaData.ParticipantNumber = _participantNumber;
                ExperimentMetaData.Environments = list;
                ExperimentMetaData.TimeInEnvironment = PlayerPrefs.GetInt("TimeSetting");
                ExperimentMetaData.StartTime = DateTime.Now;
            
                Debug.Log($"Starting with id: {_experimentId}");
                Cursor.lockState = CursorLockMode.Locked;
                SceneManager.LoadScene(1);
            }
            else
            {
                ShowErrorMessage("Please enter a positive participant number and select an experiment file!");
            }
        }

        private void FixedUpdate()
        {
            if (_errorOccured)
            {
                if (_errorEndTime < Time.time)
                {
                    var last = (int)ErrorText.faceColor.a;
                    ErrorText.faceColor = new Color32(255, 0, 0,(byte)(last - _vanishingRate));
                    if (last - _vanishingRate <= 0)
                    {
                        _errorOccured = false;
                        ErrorText.text = "";
                        ErrorText.color = Color.red;
                    }
                }
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}