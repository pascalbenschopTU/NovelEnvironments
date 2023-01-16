using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    private string layer;
    private GameObject[] objects;
    private string[] objectNames;
    private GameObject gatherable;
    private int seed;
    private int amount;
    System.Random prng;
    
    public void Initialize(string layer, GameObject[] objects, int seed, int amount, GameObject gatherable)
    {
        this.layer = layer;
        this.objects = objects;
        this.objectNames = new string[objects.Length+1];
        this.seed = seed;
        this.amount = amount;
        this.gatherable = gatherable;
        this.prng = new System.Random(seed);

        objectNames[0] = "PathMesh";
        for (int i = 0; i < objects.Length; i++) {
            objectNames[i+1] = objects[i].GetComponentInChildren<MeshFilter>().name;
        }
    }

    bool isOnTerrain(Vector3 vertex, Vector3 center, Vector3 extents)
    {
        Vector3 heightV = vertex + new Vector3(center.x, 100, center.z);
        List<Vector3> points = new List<Vector3>();
        points.Add(heightV);

        for (int i = 0; i <= extents.x+1; i+=2) {
            for (int j = 0; j <= extents.z+1; j+=2){
                Vector3 x = Vector3.right * i;
                Vector3 z = Vector3.forward * j;
                points.AddRange(new Vector3[]{heightV-x-z, heightV-x+z, heightV+x-z, heightV+x+z});
            }
        }

        for (int i = 0; i < points.Count; i++){
            RaycastHit[] hits = Physics.RaycastAll(points[i], Vector3.down*110, 110.0F);

            for (int j = 0; j < hits.Length; j++) {
                System.Console.WriteLine("hit: {0}", hits[j].collider.name);
                if (System.Array.Exists(objectNames, element => element == hits[j].collider.name))
                    return false;
            }

            if (Physics.Linecast(vertex+Vector3.down*10, vertex + Vector3.up*100))
                return false;
        }

        return true;
    }

    public void GenerateObjects(Mesh mesh, Vector3 spawn)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            // Generate #amount instances of objects[i] 
            int testVerticeIndex = prng.Next(0, mesh.vertices.Length);
            Vector3 testVertice = mesh.vertices[testVerticeIndex];

            GameObject objectBounds = objects[i];
            objectBounds.layer = LayerMask.NameToLayer(layer);
            GameObject newgo = Instantiate(objectBounds, testVertice, Quaternion.identity);
            Bounds bounds = newgo.GetComponentInChildren<Renderer>().bounds;
            Vector3 center = bounds.center-testVertice;
            Vector3 extents = bounds.extents;
            Destroy(newgo);
            int objectAmount = Mathf.Min((int)(150/(extents.x + extents.z)), amount);

            int j = 0;
            while (j < objectAmount)
            {
                int verticeIndex = prng.Next(0, mesh.vertices.Length);
                Vector3 vertice = mesh.vertices[verticeIndex];

                if ((vertice.x > spawn.x+5 || vertice.x < spawn.x-5) && (vertice.z > spawn.z+5 || vertice.z < spawn.z-5) && isOnTerrain(vertice, center, extents))
                {
                    GameObject objectToSpawn = objects[i];
                    objectToSpawn.layer = LayerMask.NameToLayer(layer);
                    Instantiate(objectToSpawn, vertice, Quaternion.identity);
                    
                    j++;
                }
            }
        }
    }

    public void GenerateGatherables(List<(Vector3 start, Vector3 end)> paths)
    {
        List<Vector3> starts = new List<Vector3>();
        int pathIndex = prng.Next(0, paths.Count);
        (Vector3 start, Vector3 end) = paths[pathIndex];
        starts.Add(start);

        for (int i = 0; i < 20; i++){

            while (starts.Contains(paths[pathIndex].start)) {
                pathIndex = prng.Next(0, paths.Count);
                (start, end) = paths[pathIndex];
            }

            starts.Add(start);

            Vector3 dist = end - start;
            float len = (float)prng.NextDouble()*0.7f;
            Vector3 pointOnPath = start + dist*0.15f + dist*len;
            Vector3 perp = new Vector3(dist.z, 0, -dist.x);
            int shift = (prng.Next(0, 1)*2-1)*4;
            Vector3 spawn = pointOnPath + perp.normalized*shift + Vector3.up*100;

            RaycastHit[] hits = Physics.RaycastAll(spawn, Vector3.down, 150.0F);

            for (int j = 0; j < hits.Length; j++) {
                if (hits[j].collider.name == "Mesh"){
                    spawn = hits[j].point;
                    break;
                }
            }

            GameObject gatherableCopy = gatherable;
            gatherableCopy.layer = LayerMask.NameToLayer(layer);
            gatherableCopy.transform.localScale = new Vector3(4, 4, 4);
            gatherableCopy.tag = "Gather";
            Instantiate(gatherableCopy, spawn, Quaternion.identity);
        }
    }
}
