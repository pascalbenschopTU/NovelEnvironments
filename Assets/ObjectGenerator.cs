using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    private string layer;
    private GameObject[] objects;
    private GameObject gatherable;
    private int seed;
    private int amount;
    System.Random prng;
    
    public void Initialize(string layer, GameObject[] objects, int seed, int amount, GameObject gatherable)
    {
        this.layer = layer;
        this.objects = objects;
        this.seed = seed;
        this.amount = amount;
        this.gatherable = gatherable;
        this.prng = new System.Random(seed);
    }

    public void GenerateObjects(Mesh mesh)
    {
        Vector3 startingPosition = new Vector3(30.0f, 1.0f, -20.0f);
        for (int i = 0; i < objects.Length; i++)
        {
            // Generate #amount instances of objects[i] 
            for (int j = 0; j < amount; j++)
            {
                int verticeIndex = prng.Next(0, mesh.vertices.Length);
                Vector3 vertice = mesh.vertices[verticeIndex];

                if ((vertice.x > startingPosition.x+5 || vertice.x < startingPosition.x-5) && (vertice.z > startingPosition.z+5 || vertice.z < startingPosition.z-5))
                {
                    GameObject objectToSpawn = objects[i];
                    objectToSpawn.layer = LayerMask.NameToLayer(layer);
                    Instantiate(objectToSpawn, vertice, Quaternion.identity);
                }
            }
        }
    }

    public void GenerateGatherables(Mesh mesh)
    {
        Vector3 startingPosition = new Vector3(30.0f, 1.0f, -20.0f);

        for (int i = 0; i < 3; i++){
            int vertIndex = prng.Next(0, mesh.vertices.Length);
            Vector3 vert = mesh.vertices[vertIndex];

            if ((vert.x > startingPosition.x+5 || vert.x < startingPosition.x-5) && (vert.z > startingPosition.z+5 || vert.z < startingPosition.z-5))
            {
                GameObject gatherableCopy = gatherable;
                gatherableCopy.layer = LayerMask.NameToLayer(layer);
                gatherableCopy.transform.localScale = new Vector3(4, 4, 4);
                gatherableCopy.tag = "Gather";
                Instantiate(gatherableCopy, vert, Quaternion.identity);
            }
        }
    }
}
