using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public GameObject[] objects;
    public GameObject[] landMarks;

    [SerializeField] private AnimationCurve heightCurve;

    public int xSize;
    public int zSize;

    public float scale;
    public int octaves;
    public float lacunarity;

    public int seed;

    public Gradient gradient;

    private float minTerrainHeight;
    private float maxTerrainHeight;
    private float lastNoiseHeight;

    // Start is called before the first frame update
    void Start()
    {
        SetNullProperties();

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateNewMap();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshCollider>().enabled = true;
    }

    private void SetNullProperties()
    {
        if (xSize <= 0) xSize = 50;
        if (zSize <= 0) zSize = 50;
        if (octaves <= 0) octaves = 5;
        if (lacunarity <= 0) lacunarity = 2;
        if (scale <= 0) scale = 50;
    }

    public void CreateNewMap()
    {
        CreateMeshShape();
        CreateTriangles();
        ColorMap();
        UpdateMesh();
        createFauna();
        addLandMarks();
    }

    private void CreateMeshShape()
    {
        // Creates seed
        Vector2[] octaveOffsets = GetOffsetSeed();

        if (scale <= 0) scale = 0.0001f;

        // Create vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // Set height of vertices
                float noiseHeight = GenerateNoiseHeight(z, x, octaveOffsets);
                SetMinMaxHeights(noiseHeight);
                vertices[i] = new Vector3(x, noiseHeight, z);
                i++;
            }
        }
    }

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

    private void SetMinMaxHeights(float noiseHeight)
    {
        // Set min and max height of map for color gradient
        if (noiseHeight > maxTerrainHeight)
            maxTerrainHeight = noiseHeight;
        if (noiseHeight < minTerrainHeight)
            minTerrainHeight = noiseHeight;
    }

    private void CreateTriangles()
    {
        // Need 6 vertices to create a square (2 triangles)
        triangles = new int[xSize * zSize * 6];

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
    }

    private void ColorMap()
    {
        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    void createFauna()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertice = vertices[i];
            if (vertice.y > 5 && vertice.y < 15)
            {
                if (Random.Range(1, 20) == 1)
                {
                    GameObject objectToSpawn = objects[Random.Range(0, objects.Length)];
                    objectToSpawn.layer = LayerMask.NameToLayer("Ground");
                    Instantiate(objectToSpawn, vertice, Quaternion.identity);
                }
            }    
        }
    }

    void addLandMarks()
    {
        for (int i = 0; i < 1; i++)
        {
            Vector3 vertice = vertices[Random.Range(0, vertices.Length)];
            GameObject objectToSpawn = landMarks[Random.Range(0, landMarks.Length)];
            vertice.y += 10;
            Instantiate(objectToSpawn, vertice, Quaternion.Euler(new Vector3(-90, 0, 0)));
        }
    }

    // Update is called once per frame
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
