using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

namespace ScriptsLogUser
{
    public class RoamingEntropy : MonoBehaviour
    {
        [SerializeField] private float EnvironmentSize;
        [SerializeField] private float AreaSizeFactorFromMeters;

        private Dictionary<int, List<PositionalData>> positionalData;
        private Dictionary<int, Array2D> roamingEntropies;

        private Array2D convolutionKernel = new Array2D(new[]
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
            
            foreach (var id in positionalData.Keys)
            {
                CalculateRoamingEntropyPerEnv(id);
            }
        }

        private void CalculateRoamingEntropyPerEnv(int environmentId)
        {
            var data = positionalData[environmentId];
            var envWidth = Mathf.RoundToInt(EnvironmentSize * AreaSizeFactorFromMeters);
            Debug.Log(envWidth);
            var array2D = new Array2D(envWidth * envWidth, envWidth, envWidth);

            foreach (var pos in data)
            {
                var x = Mathf.RoundToInt(pos.position.x * AreaSizeFactorFromMeters);
                var y = Mathf.RoundToInt(pos.position.y * AreaSizeFactorFromMeters);
                array2D.Set(array2D.Get(x, y) + 1.0f, x, y);
            }

            var entropy = array2D.PerformConvolution(convolutionKernel);
        }
    }
}

class Array2D
{
    private float[] rawArray;
    private int size;
    private int width;
    private int height;
    
    public Array2D(int size, int width, int height)
    {
        rawArray = new float[size];
        size = this.size;
        height = this.height;
        width = this.width;
    }

    public Array2D(float[] data, int width, int height)
    {
        rawArray = data;
        size = data.Length;
        width = this.width;
        height = this.height;
    }

    public void Set(float value, int x, int y)
    {
        if (x * y < size)
        {
            rawArray[y * x + x] = value;
        }
    }

    public float Get(int x, int y)
    {
        return x * y >= size ? 0.0f : rawArray[y * x + x];
    }

    public Array2D PerformConvolution(Array2D kernel)
    {
        if (Convert.ToBoolean(kernel.width % 2) || Convert.ToBoolean(kernel.height % 2))
        {
            return null;
        }
        var result = new Array2D(size, width, height);
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

        return result / kernel.size;
    }
}