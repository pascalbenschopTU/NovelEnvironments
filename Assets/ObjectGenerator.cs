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

    bool isOnTerrain(Vector3 vertex, GameObject go)
    {
        Vector3 heightV = vertex + Vector3.up*100;

        Vector3 extents = go.GetComponent<MeshCollider>().sharedMesh.bounds.extents;
        Vector3 x = Vector3.right * extents.x;
        Vector3 z = Vector3.forward * extents.z;

        Vector3[] points = {heightV, heightV-x-z, heightV-x+z, heightV+x-z, heightV+x+z};

        for (int i = 0; i < points.Length; i++){
            RaycastHit[] hits = Physics.RaycastAll(points[i], Vector3.down*110, 110.0F);

            for (int j = 0; j < hits.Length; j++) {
                if (hits[j].collider.name == "PathMesh")
                    return false;
            }

            if (Physics.Linecast(vertex+Vector3.down*10, vertex + Vector3.up*100))
                return false;
        }

        return true;
    }

    public void GenerateObjects(Mesh mesh)
    {
        Vector3 startingPosition = new Vector3(30.0f, 1.0f, -20.0f);
        for (int i = 0; i < objects.Length; i++)
        {
            // Generate #amount instances of objects[i] 
            int j = 0;
            while (j < amount)
            {
                int verticeIndex = prng.Next(0, mesh.vertices.Length);
                Vector3 vertice = mesh.vertices[verticeIndex];

                if ((vertice.x > startingPosition.x+5 || vertice.x < startingPosition.x-5) && (vertice.z > startingPosition.z+5 || vertice.z < startingPosition.z-5) && isOnTerrain(vertice, objects[i]))
                {
                    GameObject objectToSpawn = objects[i];
                    objectToSpawn.layer = LayerMask.NameToLayer(layer);
                    Instantiate(objectToSpawn, vertice, Quaternion.identity);
                    j++;
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
