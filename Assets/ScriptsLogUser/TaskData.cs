using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;

public class TaskData : PositionalData{

    public string task { get; private set; }

    public TaskData(PositionalData positionalData, string task)
        : base(positionalData.environment_id, positionalData.position, positionalData.rotation)
    {
        this.task = task;
    }


    public new string ToCSV()
    {
        return $"{this.environment_id};{position.x};{position.y};{position.z};{rotation.w};{rotation.x};{rotation.y};{rotation.z};{task}";
    }

    public new static TaskData FromCSV(string[] csvColumns)
    {
        PositionalData positionalData = PositionalData.FromCSV(csvColumns);
        return new TaskData(positionalData, csvColumns[8]);
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            TaskData t = (TaskData)obj;
            return (environment_id == t.environment_id) && 
                (position.x == t.position.x) &&
                (position.y == t.position.y) &&
                (position.z == t.position.z) &&
                (task == t.task);
        }
    }

    public override int GetHashCode()
    {
        return environment_id.GetHashCode() * 
            position.x.GetHashCode() *
            position.y.GetHashCode() *
            position.z.GetHashCode() *
            task.GetHashCode();
    }
}