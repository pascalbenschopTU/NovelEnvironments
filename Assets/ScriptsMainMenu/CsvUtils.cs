using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CsvUtils : MonoBehaviour
{
    // CSV Format: type, texture, movingobjects, interaction, fov, map
    public static Dictionary<int,List<EnvironmentConfiguration>> EnvironmentConfigsFromCsv(string path, string delimiter = ";")
    {
        var dict = new Dictionary<int,List<EnvironmentConfiguration>>();
        using var reader = new StreamReader(path);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line == null) continue;

            var values = line.Split(delimiter);
            var conf = EnvironmentConfiguration.FromCsv(values);
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

    public static bool EnvironmentConfigsToCsv(Dictionary<int,List<EnvironmentConfiguration>> configurations, string path)
    {
        using var writer = new StreamWriter(path);
        foreach (var config in configurations.SelectMany(pair => pair.Value)) // is like double for loop
        {
            writer.WriteLine(config.ToCsv());
        }

        return true;
    }

    public static Dictionary<int, List<PositionalData>> PositionalDataFromCsv(string delimiter = ";")
    {
        CreateExperimentLogsDirectoryIfNotExists();
        string path = $"{Application.dataPath}/ExperimentLogs_{ExperimentMetaData.ParticipantNumber}/movement.csv";

        if (!File.Exists(path))
        {
            return null;
        }

        var dict = new Dictionary<int, List<PositionalData>>();
        using var reader = new StreamReader(path);
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

    public static bool PositionalDataToCsv(List<PositionalData> recording)
    {
        CreateExperimentLogsDirectoryIfNotExists();
        string path = $"{Application.dataPath}/ExperimentLogs_{ExperimentMetaData.ParticipantNumber}/movement.csv";
        using var writer = new StreamWriter(path, append: true);
        foreach (PositionalData data in recording)
        {
            writer.WriteLine(data.ToCSV());
        }

        return true;
    }

    public static Dictionary<int, List<TaskData>> TaskDataFromCsv(string delimiter = ";")
    {
        CreateExperimentLogsDirectoryIfNotExists();
        string path = $"{Application.dataPath}/ExperimentLogs_{ExperimentMetaData.ParticipantNumber}/tasks.csv";
        if (!File.Exists(path))
        {
            return null;
        }

        var dict = new Dictionary<int, List<TaskData>>();
        using var reader = new StreamReader(path);
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

    public static bool TaskDataToCsv(List<TaskData> tasks)
    {
        CreateExperimentLogsDirectoryIfNotExists();
        string path = $"{Application.dataPath}/ExperimentLogs_{ExperimentMetaData.ParticipantNumber}/tasks.csv";
        using var writer = new StreamWriter(path, append: true);
        foreach (TaskData data in tasks)
        {
            writer.WriteLine(data.ToCSV());
        }

        return true;
    }

    private static void CreateExperimentLogsDirectoryIfNotExists()
    {
        var dirPath = Application.dataPath + $"/ExperimentLogs_{ExperimentMetaData.ParticipantNumber}/";
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
