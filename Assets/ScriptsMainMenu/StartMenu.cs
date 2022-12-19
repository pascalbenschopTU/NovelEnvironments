using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public TMP_Dropdown Dropdown;
    public TMP_InputField InputParticipantNumber;
    public TextMeshProUGUI ErrorText;

    public float ErrorTextTimeout;
    private int _participantNumber;
    private int _experimentId;
    private int _moduloValue;
    private bool _fileSelected;
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

    public void ToggleModulo(bool value)
    {
        _moduloActive = value;
        ChangeParticipantNumber(_participantNumber.ToString());
    }

    public void SetModuloValue(string value)
    {
        _moduloValue = int.TryParse(value, out var outVal) ? outVal: -1;
        ChangeParticipantNumber(_participantNumber.ToString());
    }

    public void LoadExperimentsFromFile()
    {
            Dropdown.ClearOptions();
        if (PlayerPrefs.HasKey("SelectedFile"))
        {
            _fileSelected = true;
            _environmentConfigurations = CsvUtils.EnvironmentConfigsFromCsv(PlayerPrefs.GetString("SelectedFile"));
            Dropdown.AddOptions(_environmentConfigurations.Keys.ToList().ConvertAll(k => k.ToString()));
            ChangeParticipantNumber(_participantNumber.ToString());
        }
        else
        {
            _fileSelected = false;
            ShowErrorMessage("Please select an experiment file in settings menu!");
        }
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
                if (_fileSelected && _environmentConfigurations.Keys.ToList().Count > _experimentId)
                {
                    // Debug.Log($"Experiment {_experimentId} selected!");
                    Dropdown.value = _experimentId;
                }else if (_fileSelected)
                {
                    // Debug.Log($"Not Enough experiments ({_environmentConfigurations.Keys.ToList().Count}) for modulo value {_moduloValue} with result {_experimentId}!");
                    _experimentId = _environmentConfigurations.Keys.ToList().Count - 1;
                    // Debug.Log($"Selected last Experiment: {_experimentId}!");
                    Dropdown.value = _experimentId;
                }
            }
        }
    }

    public void StartGame()
    {
        if (_fileSelected && _participantNumber >= 0)
        {
            // start game
            List<EnvironmentConfiguration> list = _environmentConfigurations[0];
            
            //TODO generate seed or get it from somewhere
            ExperimentMetaData.Seed = 100;
            ExperimentMetaData.ParticipantNumber = _participantNumber;
            ExperimentMetaData.Environments = list;
            ExperimentMetaData.TimeInEnvironment = PlayerPrefs.GetInt("TimeSetting");
            ExperimentMetaData.StartTime = DateTime.Now;

            Cursor.lockState = CursorLockMode.Locked;
            SceneManager.LoadScene(1);
        }
        else
        {
            // Debug.Log($"Can't start the game due to missing info!");
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
