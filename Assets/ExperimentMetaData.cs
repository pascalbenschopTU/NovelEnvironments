using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentMetaData : MonoBehaviour
{
    public static int Index = 0;
    public static List<EnvironmentConfiguration> Environments;
    public static EnvironmentConfiguration currentEnvironment;
    public static int TimeInEnvironment = 20;
    public static int Seed = 100;
    public static DateTime StartTime = DateTime.Now;
    public static DateTime EndTime = DateTime.Now;

    public static int ParticipantNumber = 0;
    public static bool ExperimentFinished = false;
    // attach the real log content or path to logging file for final dataset ?
    public static string LogDirectory="";

    public static string ToJson(ExperimentData experimentData)
    {
        var obj = new ExperimentConfig()
        {
            Seed = ExperimentMetaData.Seed,
            TimeInEnvironment = ExperimentMetaData.TimeInEnvironment,
            StartTime = ExperimentMetaData.StartTime.ToString("G"),
            EndTime = ExperimentMetaData.EndTime.ToString("G"),
            ParticipantNumber = ExperimentMetaData.ParticipantNumber,
            ExperimentFinished = ExperimentMetaData.ExperimentFinished,
            ExperimentData = experimentData
        };
        return JsonUtility.ToJson(obj);
    }
}
[Serializable]
class ExperimentConfig
{
    public int TimeInEnvironment;
    public int Seed;
    public string StartTime;
    public string EndTime;
    public int ParticipantNumber;
    public bool ExperimentFinished;
    public ExperimentData ExperimentData;
}

[Serializable]
public class ExperimentData
{
    public float DistanceWalked;
    public int LandmarksFound;
    public int LandmarksFoundMax;
    public int PicturesTaken;
    public int ObjectsPickedUp;
    public int ObjectsPickedUpMax;
    public string GameTime;
    public float RoamingEntropy = 0.0f;
}

