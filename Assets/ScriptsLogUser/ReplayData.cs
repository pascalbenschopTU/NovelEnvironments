using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayData {
    public Vector3 position {get; private set;}
    public Quaternion rotation {get; private set;}
    
    public ReplayData(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public override string ToString()
    {
        char[] charsToTrim = {'(', ')'};
        return $"{position.ToString().Trim(charsToTrim)}\n" +
               $"{rotation.ToString().Trim(charsToTrim)}";
    }
}