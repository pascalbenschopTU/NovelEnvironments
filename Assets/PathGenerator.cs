using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    Mesh mesh;
    Mesh envMesh;

    private GameObject[] landmarks;
    private int seed;
    Material terrainMaterial;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    int vertIndex, triIndex, verticesCount = 0;
    private int pathWidth = 10;
    private float pathScale = 1.0f;

    List<Vector3> landMarkCoords;

    public void Initialize(GameObject[] landmarks, int seed, Material terrainMaterial)
    {
        this.landmarks = landmarks;
        this.seed = seed;
        this.terrainMaterial = terrainMaterial;
    }

    public void GenerateLandmarks(Mesh[] meshes)
    {
        landMarkCoords = new List<Vector3>();
        System.Random prng = new System.Random(seed);
        // Generate landmarks spread over the map 
        // TODO depend on path generation
        for (int i = 0; i < landmarks.Length; i++)
        {
            //Mesh mesh = meshes[prng.Next(0, meshes.Length)];
            Mesh envMesh = meshes[0];
            int verticeIndex = prng.Next(0, envMesh.vertices.Length);

            Vector3 vertice = envMesh.vertices[verticeIndex];
            GameObject objectToSpawn = landmarks[i];

            vertice.y -= 3;
            landMarkCoords.Add(vertice);

            Instantiate(objectToSpawn, vertice, Quaternion.Euler(new Vector3(-90, 0, 0)));
        }
    }

    public void GeneratePaths(Mesh[] meshes)
    {
        mesh = new Mesh();
        envMesh = meshes[0];
        Vector3 first = new Vector3(20, 8, 20);

        foreach (Vector3 second in landMarkCoords) {

            Vector3 dist = new Vector3(second.x - first.x, 0, second.z - first.z);
            Vector3 normDist = Vector3.Normalize(dist)*pathScale;
            if (dist.x != 0.0f)
                verticesCount += (int)(dist.x/normDist.x)+1;
            else
                verticesCount += (int)(dist.z/normDist.z)+1;
            System.Console.WriteLine("first: {0} second: {1} dist {2} normDistx {3} normDistz {4} verticesCount {5}", first, second, dist, normDist.x, normDist.z, verticesCount);

            first = second;
        }

        vertices = new Vector3[(verticesCount+1) * (pathWidth+1)];
        colors = new Color[vertices.Length];
        triangles = new int[verticesCount * pathWidth * 6];

        first = new Vector3(20, 8, 20);


        foreach (Vector3 second in landMarkCoords) {

            System.Console.WriteLine("first: {0} second: {1}", first, second);
            int len = addPathVertices(first, second);
            addPathTriangles(len);

            first = second;
        }

        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.yellow;
        
        UpdateMesh();

        GameObject go = new GameObject("Mesh");
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshCollider>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<MeshRenderer>().material = terrainMaterial;
        go.GetComponent<MeshFilter>().mesh = mesh;
        go.GetComponent<MeshCollider>().sharedMesh = mesh;
        go.GetComponent<MeshCollider>().enabled = true;

        go.layer = LayerMask.NameToLayer("Ground");
    }

    int addPathVertices(Vector3 s, Vector3 f)
    {
        Vector3 pos = s;
        Vector3 dist = new Vector3(f.x - s.x, 0, f.z - s.z);
        Vector3 normDist = Vector3.Normalize(dist)*pathScale;
        float angle = Vector3.Angle(Vector3.forward, dist) + 90;
        Vector3 perp = Vector3.Normalize(new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0, Mathf.Cos(Mathf.Deg2Rad * angle)));

        int len = (int)(dist.z/normDist.z);
        if (dist.x != 0.0f)
            len = (int)(dist.x/normDist.x);
        
        for (int i = 0; i <= len; i++) {
            //pathVertices[vertIndex++] = pos;

            Vector3 start = pos - (pathWidth/2)*perp;
            for (int j = 0; j <= pathWidth; j++){
                Vector3 adjustedV = start + j*perp*pathScale;
                Vector3 heightV = envMesh.vertices[(int)(adjustedV.x)+201*(int)(adjustedV.z)];
                Vector3 vertex = new Vector3(adjustedV.x, heightV.y+0.01f, adjustedV.z);
                vertices[vertIndex++] = vertex;
                System.Console.WriteLine("vertIndex: {7} width: {8} s: {0} f: {1} adjustedV: {2} heightV: {3} dist: {4} pos: {5} vertex: {6}", s, f, adjustedV, heightV, dist, pos, vertex, vertIndex, pathWidth);
                System.Console.WriteLine("vertsize: {7} trisize: {8} s: {0} f: {1} adjustedV: {2} heightV: {3} dist: {4} pos: {5} vertex: {6}", s, f, adjustedV, heightV, dist, pos, vertex, vertices.Length, triangles.Length);
            }

            pos += normDist;
        }

        return len;
    }

    void addPathTriangles(int len)
    {
        int vert = vertIndex - (len+1)*(pathWidth+1);
        // Go to next row
        for (int z = 0; z < len; z++)
        {
            // fill row
            for (int x = 0; x < pathWidth; x++)
            {
                triangles[triIndex + 0] = vert + 0;
                triangles[triIndex + 1] = vert + pathWidth + 1;
                triangles[triIndex + 2] = vert + 1;
                triangles[triIndex + 3] = vert + 1;
                triangles[triIndex + 4] = vert + pathWidth + 1;
                triangles[triIndex + 5] = vert + pathWidth + 2;

                System.Console.WriteLine("first: {0} second: {1} dist {2} normDistx {3} normDistz {4} verticesCount {5}, pathWidth {6}", triangles[triIndex + 0], triangles[triIndex + 1], triangles[triIndex + 2], triangles[triIndex + 3], triangles[triIndex + 4], triangles[triIndex + 5], pathWidth);
                System.Console.WriteLine("vert: {0} triIndex: {1} len: {2} vertIndex: {3} pathWidth: {4}", vert, triIndex, len, vertIndex, pathWidth);
                vert++;
                triIndex += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }
}
