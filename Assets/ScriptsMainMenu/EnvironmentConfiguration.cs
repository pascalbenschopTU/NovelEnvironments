using System;
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

[Serializable]
public class EnvironmentConfiguration
{
    public ConfigType ComplexObjectConfig { get; set; }
    public ConfigType InteractionConfig { get; set; }
    public ConfigType RDConfig { get; set; }
    public ConfigType MapConfig { get; set; }
    public EnvironmentType EnvironmentType { get; set; }
    public int Index { get; set; }
    public int ExperimentId { get; set; }
    public bool PickupTask { get; set; }
    public bool CameraTask { get; set; }
    
    public int GetComplexObjectsConfigValue()
    {
        return ComplexObjectConfig switch
        {
            ConfigType.Low => 10,
            ConfigType.High => 50,
            _ => 0
        };
    }
    public int GetRDConfigValue()
    {
        return RDConfig switch
        {
            ConfigType.Low => 50,
            ConfigType.High => 200,
            _ => 0
        };
    }

    public int GetEnvironmentType()
    {
        return (int)EnvironmentType;
    }

    public EnvironmentConfiguration()
    {
        ExperimentId = 0;
        Index = 0;
        ComplexObjectConfig = ConfigType.Low;
        InteractionConfig = ConfigType.Low;
        RDConfig = ConfigType.Low;
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
               $"ComplexObject config: {ComplexObjectConfig}\n" +
               $"Interaction config: {InteractionConfig}\n" +
               $"RD config: {RDConfig}\n" +
               $"Map config: {MapConfig}\n" +
               $"Camera task: {CameraTask}\n" +
               $"Pickup task: {PickupTask}";
    }

    public string ToCsv()
    {
        return $"{ExperimentId};{Index};{EnvironmentType};{ComplexObjectConfig};{InteractionConfig};{RDConfig};{MapConfig};{CameraTask};{PickupTask}";
    }

    public static EnvironmentConfiguration FromCsv(string[] csvColumns)
    {
        if (csvColumns.Length != 9)
        {
            return null;
        }

        try
        {
            return new EnvironmentConfiguration()
            {
                ExperimentId = int.Parse(csvColumns[0]),
                Index = int.Parse(csvColumns[1]),
                EnvironmentType = StringToEnvironment(csvColumns[2]),
                ComplexObjectConfig = StringToConfig(csvColumns[3]),
                InteractionConfig = StringToConfig(csvColumns[4]),
                RDConfig = StringToConfig(csvColumns[5]),
                MapConfig = StringToConfig(csvColumns[6]),
                CameraTask = bool.Parse(csvColumns[7]),
                PickupTask = bool.Parse(csvColumns[8])
            };
        }
        catch (Exception)
        {
            return null;
        }
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