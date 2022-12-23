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
        Vector3 startingPosition = new Vector3(30.0f, 1.0f, -20.0f);
        System.Random prng = new System.Random(seed);
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
                    SetLayerOfGameObject(objectToSpawn, LayerMask.NameToLayer(layer));
                    Instantiate(objectToSpawn, vertice, Quaternion.identity);
                }
            }
        }
    }


    private void SetLayerOfGameObject(GameObject obj, LayerMask layer)
    {
        obj.layer = layer;

        foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layer;
        }
    }
}
