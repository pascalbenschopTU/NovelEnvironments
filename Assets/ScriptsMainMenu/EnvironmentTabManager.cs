using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ConfigType
{
    Null=-1,
    Low=0,
    Mid=1,
    High=2
}

public enum EnvironmentType
{
    Null=-1,
    Forest,
    Desert,
    Sea
}

public class EnvironmentConfiguration
{
    public ConfigType TextureConfig { get; set; }
    public ConfigType MovingObjectConfig { get; set; }
    public ConfigType InteractionConfig { get; set; }
    public ConfigType FOVConfig { get; set; }
    public ConfigType MapConfig { get; set; }
    public EnvironmentType EnvironmentType { get; set; }
    public int Index { get; set; }
    public int ExperimentId { get; set; }
    public bool PickupTask { get; set; }
    public bool CameraTask { get; set; }

    public EnvironmentConfiguration()
    {
        ExperimentId = 0;
        Index = 0;
        TextureConfig = ConfigType.Mid;
        MovingObjectConfig = ConfigType.Mid;
        InteractionConfig = ConfigType.Mid;
        FOVConfig = ConfigType.Mid;
        MapConfig = ConfigType.Mid;
        EnvironmentType = EnvironmentType.Forest;
        PickupTask = false;
        CameraTask = false;
    }

    public override string ToString()
    {
        return $"ExperimentId: {ExperimentId}\n" +
               $"Index: {Index}\n" +
               $"Type config: {EnvironmentType}\n" +
               $"Texture config: {TextureConfig}\n" +
               $"MovingObject config: {MovingObjectConfig}\n" +
               $"Interaction config: {InteractionConfig}\n" +
               $"FOV config: {FOVConfig}\n" +
               $"Tap config: {MapConfig}\n" +
               $"Camera task: {CameraTask}\n" +
               $"Pickup task: {PickupTask}";
    }

    public string ToCsv()
    {
        return $"{ExperimentId};{Index};{EnvironmentType};{TextureConfig};{MovingObjectConfig};{InteractionConfig};{FOVConfig};{MapConfig};{CameraTask};{PickupTask}";
    }

    public static EnvironmentConfiguration FromCsv(string[] csvColumns)
    {
        return new EnvironmentConfiguration()
        {
            ExperimentId = int.Parse(csvColumns[0]),
            Index = int.Parse(csvColumns[1]),
            EnvironmentType = StringToEnvironment(csvColumns[2]),
            TextureConfig = StringToConfig(csvColumns[3]),
            MovingObjectConfig = StringToConfig(csvColumns[4]),
            InteractionConfig = StringToConfig(csvColumns[5]),
            FOVConfig = StringToConfig(csvColumns[6]),
            MapConfig = StringToConfig(csvColumns[7]),
            CameraTask = bool.Parse(csvColumns[8]),
            PickupTask = bool.Parse(csvColumns[9])
        };
    }
    public static EnvironmentType StringToEnvironment(string input)
    {
        input = input.ToLower();

        return input switch
        {
            "forest" => EnvironmentType.Forest,
            "desert" => EnvironmentType.Desert,
            "sea" => EnvironmentType.Sea,
            _ => EnvironmentType.Null
        };
    }
    
    public static ConfigType StringToConfig(string input)
    {
        input = input.ToLower();
        return input switch
        {
            "low" => ConfigType.Low,
            "mid" => ConfigType.Mid,
            "high" => ConfigType.High,
            _ => ConfigType.Null
        };
    }
}

public class EnvironmentTabManager : MonoBehaviour
{
    public TMP_Dropdown EnvironmentTypeDropdown;
    public ToggleGroupAdv TextureToggleGroup;
    public ToggleGroupAdv MovingObjectsToggleGroup;
    public ToggleGroupAdv InteractionToggleGroup;
    public ToggleGroupAdv FOVRangeToggleGroup;
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
        UpdateToggleGroup(TextureToggleGroup, _environmentConfiguration.TextureConfig);
        UpdateToggleGroup(MovingObjectsToggleGroup, _environmentConfiguration.MovingObjectConfig);
        UpdateToggleGroup(InteractionToggleGroup, _environmentConfiguration.InteractionConfig);
        UpdateToggleGroup(FOVRangeToggleGroup, _environmentConfiguration.FOVConfig);
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
    public void UpdateTextureConfig(ConfigType config)
    {
        // Debug.Log($"Updated Texture config to {config}");
        _environmentConfiguration.TextureConfig = config;
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
    public void UpdateFOVConfig(ConfigType config)
    {
        // Debug.Log($"Updated FOV config to {config}");
        _environmentConfiguration.FOVConfig = config;
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
            TextureToggleGroup, MovingObjectsToggleGroup, InteractionToggleGroup, FOVRangeToggleGroup, MapToggleGroup
        };
        _toggleCallbacks = new List<Action<ConfigType>>
        {
            (config) => UpdateTextureConfig(config),
            (config) => UpdateMovingObjectsConfig(config),
            (config) => UpdateInteractionConfig(config),
            (config) => UpdateFOVConfig(config),
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
                        // Debug.Log(grp.GetFirstOnIndex());
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
