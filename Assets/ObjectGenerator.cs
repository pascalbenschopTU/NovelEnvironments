using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    private GameObject[] objects;
    private int seed;
    private int amount;
    
    public void Initialize(GameObject[] objects, int seed, int amount)
    {
        this.objects = objects;
        this.seed = seed;
        this.amount = amount;
    }

    public void GenerateObjects(Mesh mesh)
    {
        System.Random prng = new System.Random(seed);
        for (int i = 0; i < objects.Length; i++)
        {
            // Generate #amount instances of objects[i] 
            for (int j = 0; j < amount; j++)
            {
                int verticeIndex = prng.Next(0, mesh.vertices.Length);
                Vector3 vertice = mesh.vertices[verticeIndex];
                // TODO This range should be set in environment generation settings.
//                if (vertice.y > 5 && vertice.y < 15)
//                {
                    GameObject objectToSpawn = objects[i];
                    objectToSpawn.layer = LayerMask.NameToLayer("Ground");
                    Instantiate(objectToSpawn, vertice, Quaternion.identity);

//                }
            }
        }
    }
}
