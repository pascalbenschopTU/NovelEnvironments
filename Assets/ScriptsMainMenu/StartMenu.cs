using System.Collections.Generic;
using System.Linq;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject ButtonChooseFile;
    public GameObject ButtonChangeFile;
    public TextMeshProUGUI TextFilename;
    public TMP_Dropdown Dropdown;
    public TMP_InputField InputParticipantNumber;
    public TMP_InputField InputModulo;

    private int _participantNumber;
    private int _experimentId;
    private int _moduloValue;
    private bool _fileSelected;
    private bool _moduloActive;
    private Dictionary<int, List<EnvironmentConfiguration>> _environmentConfigurations;
    
    // Start is called before the first frame update
    private void Awake()
    {
        InputParticipantNumber.text = string.Empty;
        TextFilename.text = string.Empty;
        TextFilename.gameObject.SetActive(false);
        ButtonChangeFile.SetActive(false);
        ButtonChooseFile.SetActive(true);
        Dropdown.gameObject.SetActive(false);
        _participantNumber = -1;
        _experimentId = -1;
        _fileSelected = false;
        _moduloValue = 10;
        _moduloActive = false;
    }

    public void ToggleModulo(bool value)
    {
        _moduloActive = value;
    }

    public void SetModuloValue(string value)
    {
        _moduloValue = int.TryParse(value, out var outVal) ? outVal: -1;
    }

    public void ChangeParticipantNumber(string input)
    {
        var output = int.TryParse(input, out var outVal) ? outVal: -1;
        Debug.Log($"Changing Part. Number to {output}");
        _participantNumber = output;
        if (_participantNumber >= 0)
        {
            _experimentId = _participantNumber % _moduloValue;
            Debug.Log($"Experiment Id: {_experimentId}");
            // if (_fileSelected && _environmentConfigurations.ContainsKey(_experimentId))
            // {
            //     Dropdown.value = _experimentId;
            // }
            // TODO make dropdown value selection independent of experiment id
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
        Dropdown.gameObject.SetActive(true);
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
        Dropdown.ClearOptions();
        _fileSelected = true;
        _environmentConfigurations = CsvUtils.EnvironmentConfigsFromCsv(path);
        Dropdown.AddOptions(_environmentConfigurations.Keys.ToList().ConvertAll(k => k.ToString()));
    }

    public void StartGame()
    {
        // if (_fileSelected && _participantNumber >= 0)
        // {
        //     // start game
        //     Debug.Log($"Starting game with number: {_participantNumber}");
        // }
        // else
        // {
        //     Debug.Log($"Can't start the game due to missing info!");
        // }
        SceneManager.LoadScene();
    }
}
