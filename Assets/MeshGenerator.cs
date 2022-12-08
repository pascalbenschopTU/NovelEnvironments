using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    private GameObject[] objects;
    private GameObject[] landMarks;

    private Material terrainMaterial;

    private AnimationCurve heightCurve;

    private int xSize;
    private int zSize;

    private float scale;
    private int octaves;
    private float lacunarity;

    private int seed;

    private Gradient gradient;

    private float minTerrainHeight;
    private float maxTerrainHeight;


    public void Initialize(GameObject[] objects, GameObject[] landMarks, Material terrainMaterial, AnimationCurve heightCurve, float scale, int octaves, float lacunarity, int seed, Gradient gradient)
    {
        this.objects = objects;
        this.landMarks = landMarks;
        this.terrainMaterial = terrainMaterial;
        this.heightCurve = heightCurve;
        this.xSize = 200;
        this.zSize = 200;
        this.scale = scale;
        this.octaves = octaves;
        this.lacunarity = lacunarity;
        this.seed = seed;
        this.gradient = gradient;

        this.minTerrainHeight = 0;
        this.maxTerrainHeight = 30;

        SetNullProperties();
    }


    // Assign a value for properties if not set
    private void SetNullProperties()
    {
        if (octaves <= 0) octaves = 5;
        if (lacunarity <= 0) lacunarity = 1;
        if (scale <= 0) scale = 100;
    }

    // Create new mesh from point (xStart, zStart) until point (xStart + xSize, zStart + zSize)
    public Mesh CreateNewMesh(int xStart, int Zstart)
    {
        SetNullProperties();

        Mesh mesh = new Mesh();
 
        Vector3[] vertices = CreateMeshShape(xStart, Zstart);
        int[] triangles = CreateTriangles();
        Color[] colors = ColorMap(vertices, gradient);
        UpdateMesh(mesh, vertices, triangles, colors);

        // Instantiate new GameObject for each mesh
        GameObject go = new GameObject("Mesh");
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshCollider>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<MeshRenderer>().material = terrainMaterial;
        go.GetComponent<MeshFilter>().mesh = mesh;
        go.GetComponent<MeshCollider>().sharedMesh = mesh;
        go.GetComponent<MeshCollider>().enabled = true;

        go.layer = LayerMask.NameToLayer("Ground");

        go.transform.parent = transform;

        return mesh;
    }

    // Create the vertices for the mesh
    private Vector3[] CreateMeshShape(int xStart, int Zstart)
    {
        // Creates seed
        Vector2[] octaveOffsets = GetOffsetSeed();

        // Create vertices
        Vector3[] vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = Zstart; z <= Zstart + zSize; z++)
        {
            for (int x = xStart; x <= xStart + xSize; x++)
            {
                // Set height of vertices
                float noiseHeight = GenerateNoiseHeight(z, x, octaveOffsets);
                vertices[i] = new Vector3(x, noiseHeight, z);
                i++;
            }
        }

        return vertices;
    }

    // Create the triangles for the mesh
    private int[] CreateTriangles()
    {
        // Need 6 vertices to create a square (2 triangles)
        int[] triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        // Go to next row
        for (int z = 0; z < xSize; z++)
        {
            // fill row
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        return triangles;
    }

    // Assign a color for each vertex based on height
    private Color[] ColorMap(Vector3[] vertices, Gradient gradient)
    {
        Color[] colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }

        return colors;
    }

    // Update each mesh with the data
    void UpdateMesh(Mesh mesh, Vector3[] vertices, int[] triangles, Color[] colors)
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    // Get value from seed
    private Vector2[] GetOffsetSeed()
    {
        if (seed < 0 || seed > 1000)
            seed = Random.Range(0, 1000);

        // changes area of map
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int o = 0; o < octaves; o++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[o] = new Vector2(offsetX, offsetY);
        }
        return octaveOffsets;
    }

    // Get height for each point
    private float GenerateNoiseHeight(int z, int x, Vector2[] octaveOffsets)
    {
        float amplitude = 20;
        float frequency = 1;
        float persistence = 0.5f;
        float noiseHeight = 0;

        // loop over octaves
        for (int y = 0; y < octaves; y++)
        {
            float mapZ = z / scale * frequency + octaveOffsets[y].y;
            float mapX = x / scale * frequency + octaveOffsets[y].x;

            //The *2-1 is to create a flat floor level
            float perlinValue = (Mathf.PerlinNoise(mapZ, mapX)) * 2 - 1;
            noiseHeight += heightCurve.Evaluate(perlinValue) * amplitude;
            frequency *= lacunarity;
            amplitude *= persistence;
        }
        return noiseHeight;
    }
}
