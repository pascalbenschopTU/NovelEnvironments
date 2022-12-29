using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Aggregate
{
    private static float inf = float.PositiveInfinity;

    public static float CalculateDistance(Dictionary<int, List<PositionalData>> positionalDatas)
    {
        float totalDistance = 0.0f;
        foreach (List<PositionalData> data in positionalDatas.Values)
        {
            for (int i = 1; i < data.Count; i++)
            {
                totalDistance += Vector3.Distance(data[i - 1].position, data[i].position);
            }
        }
        
        return totalDistance;
    }
    
}
