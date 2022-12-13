using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;

public class ExperimentCreator : MonoBehaviour
{
    public GameObject TabList;
    public GameObject CreationTab;
    public GameObject EnvironmentTabTemplate;
    public GameObject ButtonLoad;
    public GameObject ButtonNew;
    public GameObject ButtonSave;
    public GameObject ButtonDelete;
    public TMP_Dropdown Dropdown;
    public Scrollbar Scrollbar;

    public static int Margin = 0;
    private static int _maxTabs = 5;
    private static int _tabWidth = 700;
    private static int _scrollWidth = 1580;
    private static int _totalWidth = (_maxTabs + 1) * Margin + _maxTabs * _tabWidth;

    private int _experimentId;
    private float _scrollOffset;
    
    private Dictionary<int,List<EnvironmentTabManager>> _environmentTabs;
    private Dictionary<int,List<EnvironmentConfiguration>> _environmentConfigurations;

    // Start is called before the first frame update
    void Awake()
    {
        ClearExperimentCreator();
    }

    public void NewExperiment()
    {
        DisableOldTabs(_experimentId);
        
        _experimentId = Math.Max(_environmentConfigurations.Keys.Count, _environmentTabs.Keys.Count);
        Debug.Log($"Creating new Experiment with id {_experimentId}");
        _environmentConfigurations.Add(_experimentId, new List<EnvironmentConfiguration>());
        _environmentTabs.Add(_experimentId, new List<EnvironmentTabManager>());
        Dropdown.AddOptions(new List<string>{_experimentId.ToString()});
        Dropdown.value = _experimentId;
        
        Debug.Log("Creation Tab enabled on New Experiment");
        CreationTab.SetActive(true);
        CreationTab.transform.position = new Vector3(350 + _environmentTabs[_experimentId].Count*700 - _scrollOffset,300,0);
    }

    public void DeleteExperiment()
    {
        if (_environmentConfigurations.Count == 0)
        {
            return;
        }
        foreach (var tab in _environmentTabs[_experimentId])
        {
            Destroy(tab.gameObject);
        }

        var lowerIds = new List<int>();
        var keys = new List<int>(_environmentConfigurations.Keys); ;
        foreach (var id in keys)
        {
            if (id > _experimentId)
            {
                if (_environmentTabs.ContainsKey(id))
                {
                    
                    foreach (var tab in _environmentTabs[id])
                    {
                        tab.GetEnvironmentConfig().ExperimentId -= 1;
                    }
                    
                    _environmentTabs[id-1] = _environmentTabs[id];
                }
                _environmentConfigurations[id-1] = _environmentConfigurations[id];
            }
            else
            {
                lowerIds.Add(id);
            }
        }

        _environmentConfigurations.Remove(_environmentConfigurations.Keys.Count - 1);
        _environmentTabs.Remove(_environmentTabs.Keys.Count - 1);
        
        if (_environmentConfigurations.Keys.Count > 0)
        {
            var nextId = 0;
            if (lowerIds.Count > 0)
            {
                nextId = lowerIds.Max(v => v);   
            }
            _experimentId = nextId;
            Dropdown.ClearOptions();
            Dropdown.AddOptions(_environmentConfigurations.Keys.ToList().ConvertAll(k => k.ToString()));
            if (Dropdown.value == nextId)
            {
                UpdateExperimentSelection(0);
            }
            else
            {
                Dropdown.value = nextId;
            }
        }
        else
        {
            ClearExperimentCreator();
        }
            
    }

    private void ClearExperimentCreator()
    {
        Dropdown.ClearOptions();
        Dropdown.value = 0;
        Scrollbar.gameObject.SetActive(false);
        Debug.Log("Creation Tab disabled on clear Exper");
        CreationTab.SetActive(false);
        CreationTab.transform.position = new Vector3(350 - _scrollOffset,630,0);
        _experimentId = -1;
        if (_environmentTabs != null)
        {
            foreach (var tab in _environmentTabs.SelectMany(exp => exp.Value))
            {
                Destroy(tab.gameObject);
            }
        }
        _environmentTabs = new Dictionary<int,List<EnvironmentTabManager>>();
        _environmentConfigurations = new Dictionary<int, List<EnvironmentConfiguration>>();
    }

    public void SaveExperimentButtonCallback()
    {
        if (_environmentConfigurations.Keys.Count == 0)
        {
            return;
        }
        var a = FileBrowser.ShowSaveDialog(paths =>
        {
            SaveExperiment(paths[0]);
        }, () => { }, FileBrowser.PickMode.Files);
    }

    private void SaveExperiment(string path)
    {
        Debug.Log($"Saving data to {path}");
        var configs = new Dictionary<int,List<EnvironmentConfiguration>>();
        foreach (var exp in _environmentTabs)
        {
            var list = exp.Value.Select(tab => tab.GetEnvironmentConfig()).ToList();
            configs.Add(exp.Key, list);
        }
        CsvUtils.EnvironmentConfigsToCsv(configs, path);
    }
    
    public void LoadExperimentButtonCallback()
    {
        var a = FileBrowser.ShowLoadDialog(paths =>
        {
            LoadExperiment(paths[0]);
        }, () => {}, FileBrowser.PickMode.Files);
    }

    private void LoadExperiment(string path)
    {
        Debug.Log($"Loading data from {path}");
        ClearExperimentCreator();
        
        _environmentConfigurations = CsvUtils.EnvironmentConfigsFromCsv(path);
        if (_environmentConfigurations.Keys.Count > 0)
        {
            Dropdown.AddOptions(_environmentConfigurations.Keys.ToList().ConvertAll(k => k.ToString()));
            // dropdown only calls callback when value changes, otherwise do it manually
            if (Dropdown.value == 0)
            {
                UpdateExperimentSelection(0);
            }
            else
            {
                Dropdown.value = 0;
            }
        }
    }

    private void DisableOldTabs(int experiment)
    {
        if (_environmentTabs.ContainsKey(experiment))
        {
            foreach (var tab in _environmentTabs[experiment])
            {
                tab.gameObject.SetActive(false);
            }
        }
    }
    
    public void UpdateExperimentSelection(int index)
    {
        var currentExperimentId = _experimentId;
        _experimentId = index;
        // disable old tabs when there where some
        DisableOldTabs(currentExperimentId);
        
        if (!_environmentTabs.ContainsKey(index))
        {
            _environmentTabs.Add(index, new List<EnvironmentTabManager>());
            foreach (var config in _environmentConfigurations[index])
            {
                CreateNewEnvironmentTab(config);
            }
        }
        else
        {
            foreach (var tab in _environmentTabs[_experimentId])
            {
                tab.gameObject.SetActive(true);
            }
        }
        if (_environmentTabs[_experimentId].Count >= _maxTabs)
        {
            CreationTab.SetActive(false);
            Debug.Log("Creation Tab disabled!");
        }
        else
        {
            Debug.Log("Creation Tab enabled!");
            CreationTab.SetActive(true);
            CreationTab.transform.position = new Vector3(350 + _environmentTabs[_experimentId].Count*700 - _scrollOffset, 300, 0);
        }
    }

    public void UpdateScrollOffset(Vector2 value)
    {
        _scrollOffset = value.x * _scrollWidth;
    }

    public void CreateNewEnvironmentTab()
    {
        CreateNewEnvironmentTab(null);
    }
    
    public void CreateNewEnvironmentTab(EnvironmentConfiguration environmentConfiguration)
    {
        if (_environmentTabs[_experimentId].Count < _maxTabs)
        {
            var obj = Instantiate(EnvironmentTabTemplate, new Vector3(350 + _environmentTabs[_experimentId].Count * 700 - _scrollOffset, 300, 0), Quaternion.identity,
                TabList.transform);
            
            var btn = obj.transform.GetChild(1).GetComponent<Button>();
            if (btn != null)
            {
                var tab = obj.GetComponent<EnvironmentTabManager>();
                tab.UpdateID(_environmentTabs[_experimentId].Count);
                tab.GetEnvironmentConfig().ExperimentId = _experimentId;
                if (environmentConfiguration != null)
                {
                    tab.SetEnvironmentConfig(environmentConfiguration);
                }
                _environmentTabs[_experimentId].Add(tab);
                
                btn.onClick.AddListener(delegate { DeleteEnvironmentTab(tab.GetEnvironmentConfig().Index); });
                CreationTab.transform.position = new Vector3(350 + _environmentTabs[_experimentId].Count*700 - _scrollOffset,630,0);
                if (_environmentTabs[_experimentId].Count == _maxTabs)
                {
                    Debug.Log("Creation Tab disabled in Creation!");
                    CreationTab.SetActive(false);
                }
                else
                {
                    Debug.Log("Creation Tab enabled in Creation!");
                    CreationTab.SetActive(true);
                }
            }
            else
            {
                Destroy(obj);
            }
        }
    }
    public void DeleteEnvironmentTab(int id)
    {
        Debug.Log($"Deleting tab {id}");
        var index = -1;
        
        for (var i = 0; i < _environmentTabs[_experimentId].Count; i++)
        {
            var tab = _environmentTabs[_experimentId][i];
            if (tab.GetEnvironmentConfig().Index == id)
            {
                if (_environmentTabs[_experimentId].Remove(tab))
                {
                    Destroy(tab.gameObject);
                    index = i;
                }
            }
        }
        Debug.Log($"Deleting tab on index {index}");

        if (index >= 0)
        {
            for (var i = index; i < _environmentTabs[_experimentId].Count; i++)
            {
                _environmentTabs[_experimentId][i].gameObject.transform.position -= new Vector3(700, 0, 0);
                _environmentTabs[_experimentId][i].UpdateID(_environmentTabs[_experimentId][i].GetEnvironmentConfig().Index);
            }
            CreationTab.transform.position = new Vector3(350 + _environmentTabs[_experimentId].Count*700 - _scrollOffset,630,0);
            Debug.Log("Creation Tab enabled on Delete tab");
            CreationTab.SetActive(true);
        }
    }
}
