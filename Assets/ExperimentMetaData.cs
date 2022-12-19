using System;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentMetaData : MonoBehaviour
{
    public static int Index = 0;

    public static DateTime StartTime = DateTime.Now;
    public static DateTime EndTime = DateTime.Now;
    
    public static int ParticipantNumber = 0;

    public static List<EnvironmentConfiguration> Environments;

    public static int TimeInEnvironment = 20;

    public static int Seed = 100;
    
    // attach the real log content or path to logging file for final dataset ?
    public static string Log="";
}