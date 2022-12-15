using System.Collections;
using UnityEngine;

public enum ConfigType
{
    Null = -1,
    Low = 0,
    High = 1
}

public enum EnvironmentType
{
    Null = -1,
    Forest = 0,
    Alien = 1,
    City = 2,
    Snow = 3
}

public class EnvironmentConfiguration
{
    public ConfigType NumberObjectsConfig { get; set; }
    public ConfigType MovingObjectConfig { get; set; }
    public ConfigType InteractionConfig { get; set; }
    public ConfigType FOVConfig { get; set; }
    public ConfigType MapConfig { get; set; }
    public EnvironmentType EnvironmentType { get; set; }
    public int Index { get; set; }
    public int ExperimentId { get; set; }
    public bool PickupTask { get; set; }
    public bool CameraTask { get; set; }

    public int GetNumberObjectsConfigValue()
    {
        return NumberObjectsConfig switch
        {
            ConfigType.Low => 20,
            ConfigType.High => 50,
            _ => 0
        };
    }
    public int GetMovingObjectsConfigValue()
    {
        return MovingObjectConfig switch
        {
            ConfigType.Low => 10,
            ConfigType.High => 50,
            _ => 0
        };
    }
    public int GetFOVConfigValue()
    {
        return FOVConfig switch
        {
            ConfigType.Low => 50,
            ConfigType.High => 90,
            _ => 0
        };
    }

    public EnvironmentConfiguration()
    {
        ExperimentId = 0;
        Index = 0;
        NumberObjectsConfig = ConfigType.Low;
        MovingObjectConfig = ConfigType.Low;
        InteractionConfig = ConfigType.Low;
        FOVConfig = ConfigType.Low;
        MapConfig = ConfigType.Low;
        EnvironmentType = EnvironmentType.Forest;
        PickupTask = false;
        CameraTask = false;
    }

    public override string ToString()
    {
        return $"ExperimentId: {ExperimentId}\n" +
               $"Index: {Index}\n" +
               $"Type config: {EnvironmentType}\n" +
               $"Texture config: {NumberObjectsConfig}\n" +
               $"MovingObject config: {MovingObjectConfig}\n" +
               $"Interaction config: {InteractionConfig}\n" +
               $"FOV config: {FOVConfig}\n" +
               $"Map config: {MapConfig}\n" +
               $"Camera task: {CameraTask}\n" +
               $"Pickup task: {PickupTask}";
    }

    public string ToCsv()
    {
        return $"{ExperimentId};{Index};{EnvironmentType};{NumberObjectsConfig};{MovingObjectConfig};{InteractionConfig};{FOVConfig};{MapConfig};{CameraTask};{PickupTask}";
    }

    public static EnvironmentConfiguration FromCsv(string[] csvColumns)
    {
        return new EnvironmentConfiguration()
        {
            ExperimentId = int.Parse(csvColumns[0]),
            Index = int.Parse(csvColumns[1]),
            EnvironmentType = StringToEnvironment(csvColumns[2]),
            NumberObjectsConfig = StringToConfig(csvColumns[3]),
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
            "alien" => EnvironmentType.Alien,
            "city" => EnvironmentType.City,
            "snow" => EnvironmentType.Snow,
            _ => EnvironmentType.Null
        };
    }

    public static ConfigType StringToConfig(string input)
    {
        input = input.ToLower();
        return input switch
        {
            "low" => ConfigType.Low,
            "high" => ConfigType.High,
            _ => ConfigType.Null
        };
    }
}