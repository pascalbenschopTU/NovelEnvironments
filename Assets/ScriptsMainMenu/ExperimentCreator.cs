using System;
using System.Collections.Generic;
using System.Linq;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScriptsMainMenu
{
    public class ExperimentCreator : MonoBehaviour
    {
        [SerializeField]
        private GameObject TabList;
        [SerializeField]
        private GameObject CreationTab;
        [SerializeField]
        private GameObject EnvironmentTabTemplate;
        [SerializeField]
        private GameObject ButtonLoad;
        [SerializeField]
        private GameObject ButtonNew;
        [SerializeField]
        private GameObject ButtonSave;
        [SerializeField]
        private GameObject ButtonDelete;
        [SerializeField]
        private TMP_Dropdown Dropdown;
        [SerializeField]
        private Scrollbar Scrollbar;

        private static int Margin = 0;
        private static int _maxTabs = 5;
        private static int _tabWidth = 700;
        private static int _scrollWidth = 1580;
        private static int _totalWidth = (_maxTabs + 1) * Margin + _maxTabs * _tabWidth;

        private int _experimentId;
        private float _scrollOffset;
        
        private Dictionary<int,List<EnvironmentTabManager>> _environmentTabs;
        private Dictionary<int,List<EnvironmentConfiguration>> _environmentConfigurations;

        void Awake()
        {
            ClearExperimentCreator();
        }

        public void NewExperiment()
        {
            DisableOldTabs(_experimentId);
            var expId = 0;
            while (_environmentConfigurations.Keys.Contains(expId) || _environmentTabs.Keys.Contains(expId))
            {
                expId++;
            }

            _experimentId = expId;
            _environmentConfigurations.Add(_experimentId, new List<EnvironmentConfiguration>());
            _environmentTabs.Add(_experimentId, new List<EnvironmentTabManager>());
            
            Dropdown.AddOptions(new List<string>{_experimentId.ToString()});
            var i = FindDropdownIndex(_experimentId);
            Dropdown.value = i > 0 ?  i : 0;

            CreationTab.SetActive(true);
            CreationTab.transform.localPosition = new Vector3(-1400 + _environmentTabs[_experimentId].Count*700,0,0);
        }

        private int FindDropdownIndex(int toFind)
        {
            for (int i = 0; i < Dropdown.options.Count; i++)
            {
                if (Dropdown.options[i].text == toFind.ToString())
                {
                    return i;
                }
            }

            return -1;
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

            // var lowerIds = new List<int>();
            // var keys = new List<int>(_environmentConfigurations.Keys);
            // foreach (var id in keys)
            // {
            //     if (id > _experimentId)
            //     {
            //         if (_environmentTabs.ContainsKey(id))
            //         {
            //         
            //             foreach (var tab in _environmentTabs[id])
            //             {
            //                 tab.GetEnvironmentConfig().ExperimentId -= 1;
            //             }
            //         
            //             _environmentTabs[id-1] = _environmentTabs[id];
            //         }
            //         _environmentConfigurations[id-1] = _environmentConfigurations[id];
            //     }
            //     else
            //     {
            //         lowerIds.Add(id);
            //     }
            // }

            _environmentConfigurations.Remove(_experimentId);
            _environmentTabs.Remove(_experimentId);

            if (_environmentConfigurations.Keys.Count > 0)
            {
                _experimentId = _environmentConfigurations.Keys.First();
                Dropdown.ClearOptions();
                Dropdown.AddOptions(_environmentConfigurations.Keys.ToList().ConvertAll(k => k.ToString()));
                var id = FindDropdownIndex(_experimentId);
                id = id > 0 ? id : 0;
                Dropdown.value = id;
                UpdateExperimentSelection(id);
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
            CreationTab.SetActive(false);
            CreationTab.transform.localPosition = new Vector3(-1400,0,0);
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
                _experimentId = 0;
                Dropdown.AddOptions(_environmentConfigurations.Keys.ToList().ConvertAll(k => k.ToString()));
                // dropdown only calls callback when value changes, otherwise do it manually
                foreach (var id in _environmentConfigurations.Keys.Where(id => !_environmentTabs.ContainsKey(id)))
                {
                    _environmentTabs.Add(id, new List<EnvironmentTabManager>());
                    foreach (var config in _environmentConfigurations[id])
                    {
                        CreateNewEnvironmentTab(config);
                        _environmentTabs[id][config.Index].gameObject.SetActive(false);
                    }
                }
                Dropdown.value = 0;
                UpdateExperimentSelection(0);
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
            var expId = int.Parse(Dropdown.options[index].text);
            var currentExperimentId = _experimentId;
            _experimentId = expId;
            // disable old tabs when there were some
            DisableOldTabs(currentExperimentId);
            
            if (!_environmentTabs.ContainsKey(expId))
            {
                _environmentTabs.Add(expId, new List<EnvironmentTabManager>());
                foreach (var config in _environmentConfigurations[expId])
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
            }
            else
            {
                CreationTab.SetActive(true);
                CreationTab.transform.localPosition = new Vector3(-1400 + _environmentTabs[_experimentId].Count*700,0,0);
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
            var expId = environmentConfiguration == null ? _experimentId : environmentConfiguration.ExperimentId;
            if (_environmentTabs[expId].Count < _maxTabs)
            {
                var obj = Instantiate(EnvironmentTabTemplate, new Vector3(-1400 + _environmentTabs[expId].Count * 700, 0, 0), Quaternion.identity,
                    TabList.transform);
                obj.transform.localPosition = new Vector3(-1400 + _environmentTabs[expId].Count * 700, 0, 0);
                var btn = obj.transform.GetChild(1).GetComponent<Button>();
                if (btn != null)
                {
                    var tab = obj.GetComponent<EnvironmentTabManager>();
                    tab.UpdateID(_environmentTabs[expId].Count);
                    tab.GetEnvironmentConfig().ExperimentId = expId;
                    if (environmentConfiguration != null)
                    {
                        tab.SetEnvironmentConfig(environmentConfiguration);
                    }
                    _environmentTabs[expId].Add(tab);
                
                    btn.onClick.AddListener(delegate { DeleteEnvironmentTab(tab.GetEnvironmentConfig().Index); });
                    CreationTab.transform.localPosition = new Vector3(-1400 + _environmentTabs[expId].Count*700,0,0);
                    if (_environmentTabs[expId].Count == _maxTabs)
                    {
                        CreationTab.SetActive(false);
                    }
                    else
                    {
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

            if (index >= 0)
            {
                for (var i = index; i < _environmentTabs[_experimentId].Count; i++)
                {
                    _environmentTabs[_experimentId][i].gameObject.transform.localPosition -= new Vector3(700, 0, 0);
                    _environmentTabs[_experimentId][i].UpdateID(_environmentTabs[_experimentId][i].GetEnvironmentConfig().Index-1);
                }
                CreationTab.transform.localPosition = new Vector3(-1400 + _environmentTabs[_experimentId].Count*700,0,0);
                CreationTab.SetActive(true);
            }
        }
    }
}
