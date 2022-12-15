using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

public class Recorder : MonoBehaviour
{
    public Queue<ReplayData> recordingQueue {get; private set;}

    private void Awake() 
    {
        recordingQueue = new Queue<ReplayData>();
    }

    public void recordReplayFrame(ReplayData data)
    {
        recordingQueue.Enqueue(data);
        Debug.Log("Recording data: " + data.position + " Rotation: " + data.rotation);
    }

    public void storeRecording()
    {
        Debug.Log("End Recording");
        string path = "./recording.csv";
        // FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);

        // using var writer = new StreamWriter(path);
        using (StreamWriter writer = new StreamWriter(path))
        {
            if(Settings.environment != null)
            {
                int environment_id = Settings.environment;
                writer.WriteLine(environment_id);
            }
            // EnvironmentGenerator env_generator = environment.GetComponent<EnvironmentGenerator>();

            foreach (ReplayData data in recordingQueue)
            {
                writer.WriteLine(data);
            }
        }
    }
}
