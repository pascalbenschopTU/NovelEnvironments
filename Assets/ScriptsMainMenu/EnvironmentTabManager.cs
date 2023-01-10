using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentTabManager : MonoBehaviour
{
    public TMP_Dropdown EnvironmentTypeDropdown;
    public ToggleGroupAdv TextureToggleGroup;
    public ToggleGroupAdv MovingObjectsToggleGroup;
    public ToggleGroupAdv InteractionToggleGroup;
    public ToggleGroupAdv RDToggleGroup;
    public ToggleGroupAdv MapToggleGroup;
    public Toggle CameraTaskToggle;
    public Toggle PickupTaskToggle;
    public TextMeshProUGUI TabHeader;

    private List<ToggleGroupAdv> _toggleGroups;
    private List<Action<ConfigType>> _toggleCallbacks;
    private EnvironmentConfiguration _environmentConfiguration;

    public void SetEnvironmentConfig(EnvironmentConfiguration configuration)
    {
        _environmentConfiguration = configuration;
        
        // update ui elements
        UpdateID(configuration.Index);
        EnvironmentTypeDropdown.value = (int)_environmentConfiguration.EnvironmentType;
        UpdateToggleGroup(TextureToggleGroup, _environmentConfiguration.NumberObjectsConfig);
        UpdateToggleGroup(MovingObjectsToggleGroup, _environmentConfiguration.MovingObjectConfig);
        UpdateToggleGroup(InteractionToggleGroup, _environmentConfiguration.InteractionConfig);
        UpdateToggleGroup(RDToggleGroup, _environmentConfiguration.RDConfig);
        UpdateToggleGroup(MapToggleGroup, _environmentConfiguration.MapConfig);
        CameraTaskToggle.isOn = _environmentConfiguration.CameraTask;
        PickupTaskToggle.isOn = _environmentConfiguration.PickupTask;
    }
    
    public EnvironmentConfiguration GetEnvironmentConfig()
    {
        return _environmentConfiguration;
    }

    public void TogglePickupTask(bool status)
    {
        // Debug.Log($"Set Pickup Task to {status}");
        _environmentConfiguration.PickupTask = status;
    }
    public void ToggleCameraTask(bool status)
    {
        // Debug.Log($"Set Camera Task to {status}");
        _environmentConfiguration.CameraTask = status;
    }
    public void UpdateEnvironmentType(int type)
    {
        // Debug.Log($"Updated Env type to {type}");
        _environmentConfiguration.EnvironmentType = (EnvironmentType)type;
    }
    public void UpdateNumberObjectConfig(ConfigType config)
    {
        // Debug.Log($"Updated Number Object config to {config} in Experiment {_environmentConfiguration.Index}");
        _environmentConfiguration.NumberObjectsConfig = config;
    }
    public void UpdateMovingObjectsConfig(ConfigType config)
    {
        // Debug.Log($"Updated Moving Object config to {config}");
        _environmentConfiguration.MovingObjectConfig = config;
    }
    public void UpdateInteractionConfig(ConfigType config)
    {
        // Debug.Log($"Updated Interaction config to {config}");
        _environmentConfiguration.InteractionConfig = config;
    }
    public void UpdateRDConfig(ConfigType config)
    {
        // Debug.Log($"Updated FOV config to {config}");
        _environmentConfiguration.RDConfig = config;
    }
    public void UpdateMapConfig(ConfigType config)
    {
        // Debug.Log($"Updated Map config to {config}");
        _environmentConfiguration.MapConfig = config;
    }

    void Awake()
    {
        var options = Enum.GetValues(typeof(EnvironmentType)).Cast<EnvironmentType>().ToList().ConvertAll(f => f.ToString());
        EnvironmentTypeDropdown.ClearOptions();
        EnvironmentTypeDropdown.AddOptions(options.GetRange(0, options.Count-1));
        EnvironmentTypeDropdown.value = 0;
        
        _environmentConfiguration = new EnvironmentConfiguration();
        _toggleGroups = new List<ToggleGroupAdv>
        {
            TextureToggleGroup, MovingObjectsToggleGroup, InteractionToggleGroup, RDToggleGroup, MapToggleGroup
        };
        _toggleCallbacks = new List<Action<ConfigType>>
        {
            (config) => UpdateNumberObjectConfig(config),
            (config) => UpdateMovingObjectsConfig(config),
            (config) => UpdateInteractionConfig(config),
            (config) => UpdateRDConfig(config),
            (config) => UpdateMapConfig(config),
        };
        
        for (var i = 0; i < _toggleGroups.Count; i++)
        {
            var grp = _toggleGroups[i];
            var func = _toggleCallbacks[i];
            foreach (var t in grp.Toggles)
            {
                t.onValueChanged.AddListener(delegate(bool value)
                {
                    if (value)
                    {
                        func((ConfigType)grp.GetFirstOnIndex());
                        func((ConfigType)grp.GetFirstOnIndex());
                    }
                });
            }
        }
        
        PickupTaskToggle.onValueChanged.AddListener(TogglePickupTask);
        CameraTaskToggle.onValueChanged.AddListener(ToggleCameraTask);
    }
    public void UpdateID(int id)
    {
        _environmentConfiguration.Index = id;
        // Debug.Log(_environmentConfiguration);
        TabHeader.text = $"Environment {id}";
    }

    public void UpdateToggleGroup(ToggleGroupAdv toggleGroupAdv, ConfigType configType)
    {
        toggleGroupAdv.Toggles[toggleGroupAdv.GetFirstOnIndex()].isOn = false;
        toggleGroupAdv.Toggles[(int)configType].isOn = true;
    }
}
