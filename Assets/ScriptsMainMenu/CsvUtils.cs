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
}
