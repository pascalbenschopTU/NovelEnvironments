using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour
{
    private MeshGenerator meshGenerator;
    private PathGenerator pathGenerator;
    private ObjectGenerator objectGenerator;

    public GameObject[] objects;
    public GameObject[] landmarks;

    public int objectAmount;

    public Material terrainMaterial;

    [SerializeField] private AnimationCurve heightCurve;

    public int xMin;
    public int yMin;

    public float scale;
    public int octaves;
    public float lacunarity;

    public int seed;

    public Gradient gradient;

    private Mesh[] meshes;
    private int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        meshGenerator = gameObject.AddComponent<MeshGenerator>();
        meshGenerator.Initialize(objects, landmarks, terrainMaterial, heightCurve, scale, octaves, lacunarity, seed, gradient);
        pathGenerator = gameObject.AddComponent<PathGenerator>(); 
        pathGenerator.Initialize(landmarks, seed, terrainMaterial);
        objectGenerator = gameObject.AddComponent<ObjectGenerator>();
        objectGenerator.Initialize(objects, seed, objectAmount);
        
        
        meshes = new Mesh[4];

        createNewEnvironment();
    }

    private void createNewEnvironment()
    {
        meshes[index++] = meshGenerator.CreateNewMesh(xMin, yMin);
        meshes[index++] = meshGenerator.CreateNewMesh(xMin, yMin + 200);
        meshes[index++] = meshGenerator.CreateNewMesh(xMin + 200, yMin);
        meshes[index++] = meshGenerator.CreateNewMesh(xMin + 200, yMin + 200);

        foreach(Mesh mesh in meshes)
        {
            objectGenerator.GenerateObjects(mesh);
        }

        pathGenerator.GenerateLandmarks(meshes);
        pathGenerator.GeneratePaths(meshes);
    }

    
}
