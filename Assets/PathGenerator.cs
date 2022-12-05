using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    private GameObject[] landmarks;
    private int seed;

    public void Initialize(GameObject[] landmarks, int seed)
    {
        this.landmarks = landmarks;
        this.seed = seed;
    }

    public void GenerateLandmarks(Mesh[] meshes)
    {
        System.Random prng = new System.Random(seed);
        // Generate landmarks spread over the map 
        // TODO depend on path generation
        for (int i = 0; i < landmarks.Length; i++)
        {
            Mesh mesh = meshes[prng.Next(0, meshes.Length)];
            int verticeIndex = prng.Next(0, mesh.vertices.Length);

            Vector3 vertice = mesh.vertices[verticeIndex];
            GameObject objectToSpawn = landmarks[i];

            vertice.y += 10;

            Instantiate(objectToSpawn, vertice, Quaternion.Euler(new Vector3(-90, 0, 0)));
        }
    }

    public void GeneratePaths(Mesh[] meshes)
    {
        return;
    }
}
