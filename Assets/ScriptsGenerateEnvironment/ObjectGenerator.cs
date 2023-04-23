using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectGenerator : MonoBehaviour
{
    private string layer;
    private GameObject[] objects;
    private GameObject[] dynamicObjects;
    private GameObject gatherable;
    private int amount = 100;
    System.Random prng;

    private int amountOfDynamicObjects = 20;

    public static GameObject[] dynamicObjectsInScene;
    
    public void Initialize(string layer, GameObject[] objects, int seed, GameObject gatherable, int amount)
    {
        this.layer = layer;
        this.objects = objects;
        this.gatherable = gatherable;
        this.amount = amount;
        this.prng = new System.Random(seed);

        dynamicObjectsInScene = new GameObject[amountOfDynamicObjects];
    }

    public void GenerateObjects(Mesh mesh, Vector3 spawn)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject objectToSpawn = objects[i];
            objectToSpawn.layer = LayerMask.NameToLayer(layer);

            Vector3 extents = GetBoundsOfObject(objectToSpawn).extents;

            // Lower amount of large structures vs higher amount of small structures
            int objectAmount = Mathf.Min((int)((amount*2)/extents.x), amount);

            int p = 0;
            int j = 0;
            // After 10 failed attempts stop spawning
            while (j < objectAmount && p < 10)
            {
                int verticeIndex = prng.Next(0, mesh.vertices.Length);
                Vector3 vertice = mesh.vertices[verticeIndex];
                if (IsNotOverlapping(vertice, extents * 1.5f))
                {
                    Instantiate(objectToSpawn, vertice, Quaternion.identity);
                    
                    j++;
                } else
                {
                    p++;
                }
            }
        }
    }

    public static Bounds GetBoundsOfObject(GameObject gameObject)
    {
        // Create a test object at 0,-100,0 and get the bounds of the entire object
        GameObject newgo = Instantiate(gameObject, Vector3.one * -100, Quaternion.identity);
        Bounds bounds = new Bounds(Vector3.one * -100, Vector3.one);
        Renderer[] renderers = newgo.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        } 
        Destroy(newgo);

        return bounds;
    }

    private bool IsNotOverlapping(Vector3 vertex, Vector3 extents)
    {
        Collider[] cols = Physics.OverlapBox(vertex, extents);
        for (int i = 0; i < cols.Length; i++)
        {
            // These may overlap
            if (cols[i].name != "Mesh" && !cols[i].name.Contains("border") && cols[i].name != "Cube")
            {
                return false;
            }
        }

        return true;
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

        GameObject gatherableExample = gatherable;
        gatherableExample.layer = LayerMask.NameToLayer(layer);
        gatherableExample.transform.localScale = new Vector3(4, 4, 4);
        gatherableExample.tag = "Gather";
        Instantiate(gatherableExample, new Vector3(15, 200.5f, -16), Quaternion.identity);
    }

    public void SpawnDynamicObjects(Mesh[] meshes, GameObject[] dynamicObjects)
    {
        this.dynamicObjects = dynamicObjects;
        StartCoroutine(SpawnDynamicObjectsAroundPlayer(meshes)); 
    }

    IEnumerator SpawnDynamicObjectsAroundPlayer(Mesh[] meshes)
    {
        while (true)
        {
            Transform player = GameObject.Find("Player").transform;
            Mesh mesh = GetPlayerMesh(meshes, player);

            for (int i = 0; i < dynamicObjectsInScene.Length; i++)
            {
                // if current index not spawned, spawn it
                if (dynamicObjectsInScene[i] == null)
                {
                    int dynamicObjectIndex = prng.Next(0, dynamicObjects.Length);
                    GameObject objectToSpawn = dynamicObjects[dynamicObjectIndex];

                    int verticeIndex = prng.Next(0, mesh.vertices.Length);
                    Vector3 vertice = mesh.vertices[verticeIndex];

                    GameObject spawnedObject = Instantiate(objectToSpawn, vertice, Quaternion.identity);

                    dynamicObjectsInScene[i] = spawnedObject;
                }
                // if object at current index is too far delete it
                else if (Vector3.Distance(dynamicObjectsInScene[i].transform.position, player.position) > 200)
                {
                    Destroy(dynamicObjectsInScene[i]);
                    dynamicObjectsInScene[i] = null;
                }
            }

            yield return new WaitForSeconds(5);
        }
    }


    private Mesh GetPlayerMesh(Mesh[] meshes, Transform player)
    {
        
        // for 600 by 600 mesh we have 3 by 3 meshes of 200 by 200
        int meshSize = (int)Math.Sqrt(meshes.Length * 1.0) * 200;
        // for x and z position (since meshes are spawned around 0,0) get fraction of length and width
        int x = (int)((player.position.x + (meshSize / 2)) / 200);
        int z = (int)((player.position.z + (meshSize / 2)) / 200);

        // Get index of current mesh, for 3 by 3 meshes its x*3+z
        int currentMeshIndex = x * (int)Math.Sqrt(meshes.Length * 1.0) + z;

        return meshes[currentMeshIndex];
    }
}
