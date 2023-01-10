using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PositionalData {

    public int environment_id {get; private set;}
    public Vector3 position {get; private set;}
    public Quaternion rotation {get; private set;}

    public PositionalData(int environment_id, Vector3 position, Quaternion rotation)
    {
        this.environment_id = environment_id;
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
        return $"{this.environment_id};{position.x};{position.y};{position.z};{rotation.w};{rotation.x};{rotation.y};{rotation.z}";
    }

    public static PositionalData FromCSV(string[] csvColumns)
    {
        int experiment_id = int.Parse(csvColumns[0]);

        var position = new Vector3()
        {
            x = float.Parse(csvColumns[1]),
            y = float.Parse(csvColumns[2]),
            z = float.Parse(csvColumns[3])
        };
        var rotation = new Quaternion()
        {
            w = float.Parse(csvColumns[4]),
            x = float.Parse(csvColumns[5]),
            y = float.Parse(csvColumns[6]),
            z = float.Parse(csvColumns[7])
        };

        return new PositionalData(experiment_id, position, rotation);
    }
}