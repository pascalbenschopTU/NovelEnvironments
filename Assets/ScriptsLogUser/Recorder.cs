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
        if (recordingQueue != null) 
        {
            recordingQueue.Enqueue(data);
        } else
        {
            recordingQueue = new Queue<ReplayData>();
            recordingQueue.Enqueue(data);
        }
    }

    public void storeRecording()
    {
        Debug.Log("End Recording");
        
    }
}
