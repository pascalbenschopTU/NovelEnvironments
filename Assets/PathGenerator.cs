using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    Mesh mesh;
    Mesh envMesh;

    private GameObject[] landmarks;
    private int seed;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    int vertIndex, triVertIndex, triIndex, verticesCount;
    private int pathWidth = 15;
    private float pathScale = 0.2f;
    Vector3 spawn = new Vector3(20, 8, 20);

    List<Vector3>[] landMarkCoords;
    Material mat;

    public void Initialize(GameObject[] landmarks, int seed, Material mat)
    {
        this.landmarks = landmarks;
        this.seed = seed;
        this.landMarkCoords = new List<Vector3>[4];
        this.mat = mat;
        for (int i = 0; i < landMarkCoords.Length; i++)
            landMarkCoords[i] = new List<Vector3>();
    }

    public void GenerateLandmarks(Mesh[] meshes)
    {
        System.Random prng = new System.Random(seed);
        // Generate landmarks spread over the map 
        // TODO depend on path generation
        //for (int i = 0; i < meshes.Length; i++)
        int i = 0;
        {    
            for (int j = 0; j < landmarks.Length; j++)
            {
                //Mesh mesh = meshes[prng.Next(0, meshes.Length)];
                Mesh landMesh = meshes[i];
                int verticeIndex = prng.Next(0, landMesh.vertices.Length);
                //int verticeIndex = Random.Range(0, landMesh.vertices.Length);

                Vector3 vertice = landMesh.vertices[verticeIndex];
                GameObject objectToSpawn = landmarks[j];

                vertice.y -= 3;
                landMarkCoords[i].Add(vertice);
                System.Console.WriteLine("verticeIndex: {0} vertice: {1} i {2} landMesh.vertices.Length {3}", verticeIndex, vertice, i, landMesh.vertices.Length);

                Instantiate(objectToSpawn, vertice, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }
    }

    void getVerticesCount(int i)
    {
        Vector3 first = spawn;

        foreach (Vector3 second in landMarkCoords[i]) 
        {
            Vector3 dist = new Vector3(second.x - first.x, 0, second.z - first.z);
            Vector3 normDist = Vector3.Normalize(dist)*pathScale;

            if (dist.x != 0.0f)
                verticesCount += (int)(dist.x/normDist.x);
            else
                verticesCount += (int)(dist.z/normDist.z);

            //System.Console.WriteLine("first: {0} second: {1} dist {2} normDistx {3} normDistz {4} verticesCount {5}", first, second, dist, normDist.x, normDist.z, verticesCount);

            first = second;
        }
    }

    public void GeneratePaths(Mesh[] meshes)
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        //for (int i = 0; i < meshes.Length; i++)
        int i = 0;
        {    
            vertIndex = triVertIndex = triIndex = verticesCount = 0;
            envMesh = meshes[i];
            getVerticesCount(i);

            vertices = new Vector3[(verticesCount+landMarkCoords[i].Count) * (pathWidth+1)];
            colors = new Color[vertices.Length];
            triangles = new int[verticesCount * pathWidth * 6];

            Vector3 first = spawn;

            foreach (Vector3 second in landMarkCoords[i]) 
            {
                int len = addPathVertices(first, second);
                addPathTriangles(len);
                triVertIndex += pathWidth+1;

                first = second;
            }
            
            UpdateMesh();
            createMeshGO();
        }
    }

    int addPathVertices(Vector3 s, Vector3 f)
    {
        Vector3 pos = s;
        Vector3 dist = new Vector3(f.x - s.x, 0, f.z - s.z);
        Vector3 normDist = dist.normalized*pathScale;
        int layerMask = 1 << 8;

        Vector3 perp = new Vector3(dist.z, 0, -dist.x).normalized;
        Color brown = new Color(0.4f, 0.3f, 0.0f, 0.5f);

        int len = (int)(dist.z/normDist.z);

        if (dist.x != 0.0f)
            len = (int)(dist.x/normDist.x);
        
        for (int i = 0; i <= len; i++) 
        {
            Vector3 start = pos - (pathWidth/2)*perp*pathScale;
            RaycastHit hit;
            Vector3 heightV;
/*
            for (int j = 0; j <= pathWidth/2; j++)
            {
                Vector3 adjustedV = start + j*perp*pathScale + Vector3.up*30;

                if (Physics.Linecast(adjustedV, adjustedV + Vector3.down*50, out hit)) {
                    heightV = hit.point;
                    System.Console.WriteLine("hit: {0} heightV: {1} adjustedV + Vector3.down: {2} adjustedV: {3} start + j*perp*pathScale: {4} pos: {5} Vector3.up*30: {6}", hit.point, heightV, adjustedV + Vector3.down*50, adjustedV, start + j*perp*pathScale, pos, Vector3.up*30);
                } else {
                    heightV = envMesh.vertices[(int)(adjustedV.x)+201*(int)(adjustedV.z)];
                    System.Console.WriteLine("did not hit: {0} heightV: {1} adjustedV + Vector3.down: {2} adjustedV: {3} start + j*perp*pathScale: {4} pos: {5} Vector3.up*30: {6}", hit.point, heightV, adjustedV + Vector3.down*50, adjustedV, start + j*perp*pathScale, pos, Vector3.up*30);
                }

                Vector3 vertex = new Vector3(adjustedV.x, heightV.y+0.01f*j, adjustedV.z);
                colors[vertIndex] = Color.Lerp();
                vertices[vertIndex++] = vertex;
            }
*/
            for (int j = 0; j <= pathWidth; j++)
            {
                Vector3 adjustedV = start + j*perp*pathScale + Vector3.up*30;

                if (Physics.Linecast(adjustedV, adjustedV + Vector3.down*50, out hit, layerMask)) {
                    heightV = hit.point;
                    //Debug.DrawRay(adjustedV, Vector3.down*hit.distance, Color.green, 5000f);
                    //System.Console.WriteLine("hit: {0} adjustedV + Vector3.down: {1} start + j*perp*pathScale: {2}", hit.point, adjustedV + Vector3.down*50, start + j*perp*pathScale);
                } else {
                    heightV = envMesh.vertices[(int)(adjustedV.x)+201*(int)(adjustedV.z)];
                    System.Console.WriteLine("did not hit: {0} adjustedV + Vector3.down: {1} start + j*perp*pathScale: {2}", hit.point, adjustedV + Vector3.down*50, start + j*perp*pathScale);
                }

                Vector3 vertex = new Vector3(adjustedV.x, heightV.y+0.025f, adjustedV.z);
                colors[vertIndex] = brown;
                vertices[vertIndex++] = vertex;
                //System.Console.WriteLine("i: {0} j: {1} s: {2} f: {3} dist: {4} vertex: {5} pos: {6} vertIndex: {7} ", i, j, s, f, dist, vertex, pos, vertIndex);
                //System.Console.WriteLine("vertsize: {0} trisize: {1} len: {2} verticesCount: {3} normDist {4} normDist.x {5} normDist.z {5} dist.normalized {6}", vertices.Length, triangles.Length, len, verticesCount, normDist, normDist.x, normDist.z, dist.normalized.x);
            }
            System.Console.WriteLine("i: {0} s: {1} f: {2} dist: {3} pos: {4} vertIndex: {5} ", i, s, f, dist, pos, vertIndex);
            System.Console.WriteLine("vertsize: {0} trisize: {1} len: {2} verticesCount: {3} normDist {4}", vertices.Length, triangles.Length, len, verticesCount, normDist);

            pos += normDist;
        }

        return len;
    }

    void addPathTriangles(int len)
    {
        //int vert = vertIndex - (len+1)*(pathWidth+1);
        // Go to next row
        for (int z = 0; z < len; z++)
        {
            // fill row
            for (int x = 0; x < pathWidth; x++)
            {
                triangles[triIndex + 0] = triVertIndex + 0;
                triangles[triIndex + 1] = triVertIndex + pathWidth + 1;
                triangles[triIndex + 2] = triVertIndex + 1;
                triangles[triIndex + 3] = triVertIndex + 1;
                triangles[triIndex + 4] = triVertIndex + pathWidth + 1;
                triangles[triIndex + 5] = triVertIndex + pathWidth + 2;

                System.Console.WriteLine("triIndex + 0: {0} triIndex + 1: {1} triIndex + 2 {2} triIndex + 3 {3} triIndex + 4 {4} triIndex + 5 {5}, pathWidth {6}", triangles[triIndex + 0], triangles[triIndex + 1], triangles[triIndex + 2], triangles[triIndex + 3], triangles[triIndex + 4], triangles[triIndex + 5], pathWidth);
                System.Console.WriteLine("vert: {0} triIndex: {1} len: {2} vertIndex: {3} pathWidth: {4} vertsize: {5} trisize: {6} vert count {7}", triVertIndex, triIndex, len, vertIndex, pathWidth, vertices.Length, triangles.Length, verticesCount);
                System.Console.WriteLine("vert + pathWidth + 1: {0} vert + 1: {1} vert + pathWidth + 2: {2} vert: {3}", vertices[triVertIndex + pathWidth + 1], vertices[triVertIndex + 1], vertices[triVertIndex + pathWidth + 2], vertices[triVertIndex]);
                triVertIndex++;
                triIndex += 6;
            }
            triVertIndex++;
        }
    }
/*
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        for (int i = 0; i < vertices.Length; i += 20)
            Gizmos.DrawSphere(vertices[i], 0.5f);
    }
*/
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    void createMeshGO()
    {
        GameObject go = new GameObject("Mesh");

        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshCollider>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<MeshRenderer>().material = mat;
        go.GetComponent<MeshFilter>().mesh = mesh;
        go.GetComponent<MeshCollider>().sharedMesh = mesh;
        go.GetComponent<MeshCollider>().enabled = true;

        go.layer = LayerMask.NameToLayer("Ground");
    }
}
