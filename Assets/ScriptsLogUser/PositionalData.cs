using System;
using System.Globalization;
using UnityEngine;

public class PositionalData {

    public int environment_id {get; private set;}
    public DateTime dateTime {get; private set;}
    public Vector3 position {get; private set;}
    public Quaternion rotation {get; private set;}

    public PositionalData(int environment_id, DateTime dateTime, Vector3 position, Quaternion rotation)
    {
        this.environment_id = environment_id;
        this.dateTime = dateTime;
        this.position = position;
        this.rotation = rotation;
    }

    public override string ToString()
    {
        char[] charsToTrim = {'(', ')'};
        return $"{position.ToString().Trim(charsToTrim)}\n" +
               $"{rotation.ToString().Trim(charsToTrim)}";
    }

    public string ToCSV()
    {
        return $"{environment_id};{dateTime.ToString("dd-MM-yyyy HH:mm:ss")};{position.x};{position.y};{position.z};{rotation.w};{rotation.x};{rotation.y};{rotation.z}";
    }

    public static PositionalData FromCSV(string[] csvColumns)
    {
        int experiment_id = int.Parse(csvColumns[0]);

        DateTime dateTime = DateTime.ParseExact(csvColumns[1], "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

        var position = new Vector3()
        {
            x = float.Parse(csvColumns[2]),
            y = float.Parse(csvColumns[3]),
            z = float.Parse(csvColumns[4])
        };
        var rotation = new Quaternion()
        {
            w = float.Parse(csvColumns[5]),
            x = float.Parse(csvColumns[6]),
            y = float.Parse(csvColumns[7]),
            z = float.Parse(csvColumns[8])
        };

        return new PositionalData(experiment_id, dateTime, position, rotation);
    }
}