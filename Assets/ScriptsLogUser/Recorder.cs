using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Recorder : MonoBehaviour
{
    public static List<PositionalData> recording {get; private set;}
    public static List<TaskData> tasks { get; private set; }

    private static bool isRecording = false;

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

    public static void StartRecording()
    {
        isRecording = true;
    }

    public static void StopRecording()
    {
        isRecording = false;
    }

    public static void RecordPlayerData(PositionalData data)
    {
        if (isRecording)
        {
            if (recording != null)
            {
                recording.Add(data);
            }
            else
            {
                recording = new List<PositionalData>
                {
                    data
                };
            }
        }
    }

    public static void RecordTaskData(TaskData data)
    {
        if (isRecording)
        {
            if (tasks != null && !tasks.Any(task => task.Equals(data)))
            {
                tasks.Add(data);
            }
            else if (tasks == null)
            {
                tasks = new List<TaskData>()
                {
                    data
                };
            }
        }
    }
}
