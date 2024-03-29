using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;

public class TaskData : PositionalData{

    public PositionalData positionalData { get; private set; }
    public string task { get; private set; }

    public TaskData(PositionalData positionalData, string task)
        : base(positionalData.environment_id, positionalData.dateTime, positionalData.position, positionalData.rotation)
    {
        this.positionalData = positionalData;
        this.task = task;
    }

    public new static string GetHeader()
    {
        return PositionalData.GetHeader() + ";Task";
    }

    public new string ToCSV()
    {
        return $"{positionalData.ToCSV()};{task}";
    }

    public new static TaskData FromCSV(string[] csvColumns)
    {
        return new TaskData(PositionalData.FromCSV(csvColumns), csvColumns[9]);
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