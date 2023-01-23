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

    public static string GetHeader()
    {
        return "Environment id;Date and Time;Position:X;Position:Y;Position:Z;Rotation:W;Rotation:X;Rotation:Y;Rotation:Z";
    }

    public string ToCSV()
    {
        return $"{environment_id};{dateTime.ToString("dd-MM-yyyy HH:mm:ss")};{position.x};{position.y};{position.z};{rotation.w};{rotation.x};{rotation.y};{rotation.z}";
    }

    public static PositionalData FromCSV(string[] csvColumns)
    {
        int experiment_id = int.Parse(csvColumns[0]);

        DateTime dateTime = ParseDateTime(csvColumns[1]);

        var position = new Vector3()
        {
            x = float.Parse(csvColumns[2].Replace(",", ".")),
            y = float.Parse(csvColumns[3].Replace(",", ".")),
            z = float.Parse(csvColumns[4].Replace(",", "."))
        };
        var rotation = new Quaternion()
        {
            w = float.Parse(csvColumns[5].Replace(",", ".")),
            x = float.Parse(csvColumns[6].Replace(",", ".")),
            y = float.Parse(csvColumns[7].Replace(",", ".")),
            z = float.Parse(csvColumns[8].Replace(",", "."))
        };

        return new PositionalData(experiment_id, dateTime, position, rotation);
    }

    private static DateTime ParseDateTime(string dateTime)
    {
        DateTime result;
        string[] formats = { "dd-MM-yyyy HH:mm:ss", "d-MM-yyyy HH:mm:ss", "dd-M-yyyy HH:mm:ss", "d-M-yyyy HH:mm:ss" };
        
        if (DateTime.TryParseExact(dateTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        {
            return result;
        }
        else if (DateTime.TryParse(dateTime, out result))
        {
            return result;
        }
        else
        {
            return DateTime.Now;
        }
    }
}