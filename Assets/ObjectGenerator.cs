using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    private string layer;
    private GameObject[] objects;
    private int seed;
    private int amount;
    
    public void Initialize(string layer, GameObject[] objects, int seed, int amount)
    {
        this.layer = layer;
        this.objects = objects;
        this.seed = seed;
        this.amount = amount;
    }

    public void GenerateObjects(Mesh mesh, GameObject temp)
    {
        System.Random prng = new System.Random(seed);
        for (int i = 0; i < objects.Length; i++)
        {
            // Generate #amount instances of objects[i] 
            for (int j = 0; j < amount; j++)
            {
                int verticeIndex = prng.Next(0, mesh.vertices.Length);
                Vector3 vertice = mesh.vertices[verticeIndex];

                GameObject objectToSpawn = objects[i];
                objectToSpawn.layer = LayerMask.NameToLayer(layer);
                Instantiate(objectToSpawn, vertice, Quaternion.identity);
            }
        }
    }
}
