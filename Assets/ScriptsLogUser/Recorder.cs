using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

public class Recorder : MonoBehaviour
{
    public static List<PositionalData> recording {get; private set;}
    public static List<TaskData> tasks { get; private set; }

    public static void ResetRecordings()
    {
        if (recording != null && tasks != null)
        {
            recording.Clear();
            tasks.Clear();
        } else
        {
            recording = new List<PositionalData>();
            tasks = new List<TaskData>();
        }
    }

    public static void RecordPlayerData(PositionalData data)
    {
        if (recording != null) 
        {
            recording.Add(data);
        } else
        {
            recording = new List<PositionalData>
            {
                data
            };
        }
    }

    public static void RecordTaskData(TaskData data)
    {
        if (tasks == null)
        {
            tasks = new List<TaskData>()
            {
                data
            };
        }
        else if (data != null && !tasks.Contains(data))
        {
            tasks.Add(data);
        } 
        
    }
}
