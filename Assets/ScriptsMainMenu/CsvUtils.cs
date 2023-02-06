using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class CsvUtils : MonoBehaviour
{
    // CSV Format: type, texture, movingobjects, interaction, fov, map
    public static Dictionary<int,List<EnvironmentConfiguration>> LoadEnvironmentConfigsFromCsv(string path, string delimiter = ";")
    {
        var dict = new Dictionary<int,List<EnvironmentConfiguration>>();
        if (!File.Exists(path))
        {
            return dict;
        }
        using var reader = new StreamReader(path);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line == null) continue;

            var values = line.Split(delimiter);
            var conf = EnvironmentConfiguration.FromCsv(values);
            if (conf == null)
            {
                Debug.Log("Config was not in the right format!");
                return new Dictionary<int, List<EnvironmentConfiguration>>();
            }
            if (dict.ContainsKey(conf.ExperimentId))
            {
                dict[conf.ExperimentId].Add(conf);
            }
            else
            {
                dict.Add(conf.ExperimentId, new List<EnvironmentConfiguration>{conf});
            }
        }

        return dict;
    }
    public static Dictionary<int, List<TaskData>> LoadTaskDataFromCsv(string directoryName, string delimiter = ";")
    {
        CreateExperimentLogsDirectoryIfNotExists(directoryName);
        var path = Path.Join(directoryName, "TaskData.csv");
        
        if (!File.Exists(path))
        {
            return null;
        }

        var dict = new Dictionary<int, List<TaskData>>();
        using var reader = new StreamReader(path);

        reader.ReadLine(); // Header
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line == null) continue;

            var values = line.Split(delimiter);
            TaskData data = TaskData.FromCSV(values);
            if (dict.ContainsKey(data.environment_id))
            {
                dict[data.environment_id].Add(data);
            }
            else
            {
                dict.Add(data.environment_id, new List<TaskData> { data });
            }
        }

        return dict;
    }
    public static Dictionary<int, List<PositionalData>> LoadPositionalDataFromCsv(string directoryName, string delimiter = ";")
    {
        CreateExperimentLogsDirectoryIfNotExists(directoryName);
        var path = Path.Join(directoryName, "MovementData.csv");

        if (!File.Exists(path))
        {
            return null;
        }

        var dict = new Dictionary<int, List<PositionalData>>();
        using var reader = new StreamReader(path);

        reader.ReadLine(); // Header
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line == null) continue;

            var values = line.Split(delimiter);
            PositionalData data = PositionalData.FromCSV(values);
            if (dict.ContainsKey(data.environment_id))
            {
                dict[data.environment_id].Add(data);
            }
            else
            {
                dict.Add(data.environment_id, new List<PositionalData> { data });
            }
        }

        return dict;
    }
    public static bool SaveEnvironmentConfigsToCsv(Dictionary<int,List<EnvironmentConfiguration>> configurations, string path)
    {
        using var writer = new StreamWriter(path);
        foreach (var config in configurations.SelectMany(pair => pair.Value)) // is like double for loop
        {
            writer.WriteLine(config.ToCsv());
        }

        return true;
    }
    public static bool SavePositionalDataToCsv(List<PositionalData> recording, string directoryPath)
    {
        CreateExperimentLogsDirectoryIfNotExists(directoryPath);
        var path = Path.Join(directoryPath, $"MovementData.csv");
        using var writer = new StreamWriter(path, append: true);

        writer.WriteLine(PositionalData.GetHeader());
        foreach (PositionalData data in recording)
        {
            writer.WriteLine(data.ToCSV());
        }

        return true;
    }
    public static bool SaveTaskDataToCsv(List<TaskData> tasks, string directoryPath)
    {
        CreateExperimentLogsDirectoryIfNotExists(directoryPath);

        var path = Path.Join(directoryPath, $"TaskData.csv");
        using var writer = new StreamWriter(path, append: true);

        writer.WriteLine(TaskData.GetHeader());
        foreach (TaskData data in tasks)
        {
            writer.WriteLine(data.ToCSV());
        }

        return true;
    }
    public static bool SaveExperimentData(string directoryPath, ExperimentData experimentData)
    {
        if (!Directory.Exists(directoryPath))
        {
            return false;
        }
        // save experiment results in json file
        string jsonData = ExperimentMetaData.ToJson(experimentData);
        File.WriteAllText(Path.Join(directoryPath, "ExperimentMetaData.json"), jsonData);
        
        // save used environment configurations in csv file
        var csvString = new StringBuilder();
        foreach (var config in ExperimentMetaData.Environments)
        {
            csvString.Append($"{config.ToCsv()}\n");
        }
        File.WriteAllText(Path.Join(directoryPath, "EnvironmentConfigurations.csv"), csvString.ToString());
        
        return true;
    }
    private static void CreateExperimentLogsDirectoryIfNotExists(string dirPath)
    {
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }
    public static Dictionary<int, List<PositionalData>> PositionalReplayDataFromCsv(string delimiter = ";")
    {
        CreateReplayLogsDirectoryIfNotExists();
        string path = $"{Application.dataPath}/ReplayData/movement.csv";

        if (!File.Exists(path))
        {
            return null;
        }

        var dict = new Dictionary<int, List<PositionalData>>();
        using var reader = new StreamReader(path);

        reader.ReadLine(); // Header
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line == null) continue;

            var values = line.Split(delimiter);
            PositionalData data = PositionalData.FromCSV(values);
            if (dict.ContainsKey(data.environment_id))
            {
                dict[data.environment_id].Add(data);
            }
            else
            {
                dict.Add(data.environment_id, new List<PositionalData> { data });
            }
        }

        return dict;
    }
    private static void CreateReplayLogsDirectoryIfNotExists()
    {
        var dirPath = Application.dataPath + $"/ReplayData/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }
}
