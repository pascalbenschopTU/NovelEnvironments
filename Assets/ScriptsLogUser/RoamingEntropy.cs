using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Internal;
using Directory = UnityEngine.Windows.Directory;

namespace ScriptsLogUser
{
    public class RoamingEntropy : MonoBehaviour
    {
        [SerializeField][Range(10,10000)] private float EnvironmentSize;
        [SerializeField][Range(0.1f,100.0f)] private float AreaSizeFactorFromMeters;

        private Dictionary<int, List<PositionalData>> positionalData;
        private Dictionary<int, Array2D> roamingEntropies;

        private Array2D convolutionKernel = new (new[]
        {
            0.0f,0.0f,0.0f,0.0f,1e-05f,1e-05f,0.0f,0.0f,0.0f,0.0f,
            0.0f,0.0f,2e-05f,0.00011f,0.00031f,0.00031f,0.00011f,2e-05f,0.0f,0.0f,
            0.0f,2e-05f,0.00031f,0.00227f,0.00617f,0.00617f,0.00227f,0.00031f,2e-05f,0.0f,
            0.0f,0.00011f,0.00227f,0.01677f,0.0456f,0.0456f,0.01677f,0.00227f,0.00011f,0.0f,
            1e-05f,0.00031f,0.00617f,0.0456f,0.12395f,0.12395f,0.0456f,0.00617f,0.00031f,1e-05f,
            1e-05f,0.00031f,0.00617f,0.0456f,0.12395f,0.12395f,0.0456f,0.00617f,0.00031f,1e-05f,
            0.0f,0.00011f,0.00227f,0.01677f,0.0456f,0.0456f,0.01677f,0.00227f,0.00011f,0.0f,
            0.0f,2e-05f,0.00031f,0.00227f,0.00617f,0.00617f,0.00227f,0.00031f,2e-05f,0.0f,
            0.0f,0.0f,2e-05f,0.00011f,0.00031f,0.00031f,0.00011f,2e-05f,0.0f,0.0f,
            0.0f,0.0f,0.0f,0.0f,1e-05f,1e-05f,0.0f,0.0f,0.0f,0.0f
        }, 10, 10);

        public void CalculateRoamingEntropy(string logDirectory)
        {
            if (!Directory.Exists(logDirectory))
            {
                return;
            }

            roamingEntropies = new Dictionary<int, Array2D>();
            positionalData = CsvUtils.LoadPositionalDataFromCsv(logDirectory);
            // var results = new List<RoamingEntropyResult>();
            
            foreach (var id in positionalData.Keys)
            {
                var res = CalculateRoamingEntropyPerEnv(id);
                res.ToCsv(Path.Join(logDirectory, $"RoamingEntropy_Env_{id}.csv"));
                // results.Add(new RoamingEntropyResult
                // {
                //     
                // });
            }
        }

        private Array2D CalculateRoamingEntropyPerEnv(int environmentId)
        {
            var data = positionalData[environmentId];
            var envWidth = Mathf.RoundToInt(EnvironmentSize * AreaSizeFactorFromMeters);
            var array2D = new Array2D(envWidth, envWidth);
            foreach (var pos in data)
            {
                var x = Mathf.RoundToInt(Mathf.Floor(pos.position.x * AreaSizeFactorFromMeters + envWidth / 2));
                var y = Mathf.RoundToInt(Mathf.Floor(pos.position.z * AreaSizeFactorFromMeters + envWidth / 2));
                array2D.Set(array2D.Get(x, y) + 1.0f, x, y);
            }
            return array2D.PerformConvolution(convolutionKernel);
        }
    }
}

// [Serializable]
// public class RoamingEntropyResult
// {
//     public int ParticipantNumber = 0;
//     public EnvironmentType EnvironmentType = EnvironmentType.Forest;
//     public int EnvironmentIndex = 0;
//     public float ExplorationRate = 0.0f;
//     public float DistanceWalked = 0.0f;
//     public float HighestRevisitScore = 0.0f;
// }
class Array2D
{
    private float[] rawArray;
    private int size;
    private int width;
    private int height;
    
    public Array2D(int width, int height)
    {
        size = width * height;
        rawArray = new float[size];
        this.height = height;
        this.width = width;
    }

    public Array2D(float[] data, int width, int height)
    {
        rawArray = data;
        this.size = width*height;
        this.width = width;
        this.height = height;
    }

    public void Set(float value, int x, int y)
    {
        if (x < width && y < height)
        {
            rawArray[y * width + x] = value;
        }
    }

    public float Get(int x, int y)
    {
        return x < width && y < height ? rawArray[y * width + x] : 0.0f;
    }

    public void ToCsv(string path)
    {
        Debug.Log("ToCsv...");
        using var writer = new StreamWriter(path);
        var csv = new List<string>();
        for (int x = 0; x < width; x++)
        {
            var line = new StringBuilder();
            for (int y = 0; y < height; y++)
            {
                line.Append($"{Get(x, y)};");
            }
            line.Remove(line.Length-1, 1);
            csv.Add(line.ToString());
        }
        foreach (var l in csv)
        {
            writer.WriteLine(l);
        }
    }

    public Array2D PerformConvolution(Array2D kernel)
    {
        if (Convert.ToBoolean(kernel.width % 2) || Convert.ToBoolean(kernel.height % 2))
        {
            return null;
        }
        var result = new Array2D(width, height);
        for (var srcX = kernel.width / 2; srcX < width - kernel.width / 2; srcX++)
        {
            for (var srcY = kernel.height / 2; srcY < height - kernel.height; srcY++)
            {
                result.Set(SingleConvolution(srcX, srcY, kernel), srcX, srcY);
            }
        }

        return result;
    }

    private float SingleConvolution(int srcX, int srcY, Array2D kernel)
    {
        var result = 0.0f;
        for (var x = -kernel.width / 2; x <= kernel.width / 2; x++)
        {
            for (var y = -kernel.height / 2; y <= kernel.height / 2; y++)
            {
                result += Get(srcX + x, srcY + y) * kernel.Get(x + kernel.width / 2, y + kernel.height / 2);
            }
        }

        return result;
    }
}