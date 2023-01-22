using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnvironmentGenerator : MonoBehaviour
{

    private EnvironmentConfiguration environmentConfiguration;
    private MeshGenerator meshGenerator;
    private PathGenerator pathGenerator;
    private ObjectGenerator objectGenerator;

    public string layer = "Ground";

    public GameObject[] objects;
    public GameObject[] complexObjects;
    public GameObject[] landmarks;
    public GameObject gatherable;

    public string meshTag;

    public int objectAmount;
    public int[] spawnCountPerObject;

    public Material terrainMaterial;

    public Material wallMaterial;

    [SerializeField] private AnimationCurve heightCurve;

    public float scale;
    public int octaves;
    public float lacunarity;

    public Gradient gradient;

    private int size = 600;

    private int xMin;
    private int zMin;

    private bool generateGatherables = false;

    private Mesh[] meshes;

    [SerializeField] private AudioSource AudioSrc = default;
    [SerializeField] private AudioClip Environment1 = default;
    [SerializeField] private AudioClip Environment2 = default;
    [SerializeField] private AudioClip Environment3 = default;
    [SerializeField] private AudioClip Environment4 = default;

    public void Initialize()
    {
        environmentConfiguration = ExperimentMetaData.currentEnvironment;

        int seed = ExperimentMetaData.Seed;
        xMin = zMin = -(size/2);

        meshGenerator = gameObject.AddComponent<MeshGenerator>();
        meshGenerator.Initialize(layer, terrainMaterial, heightCurve, scale, octaves, lacunarity, seed, gradient);
        pathGenerator = gameObject.AddComponent<PathGenerator>();
        pathGenerator.Initialize(layer, landmarks, seed, terrainMaterial, size);
        objectGenerator = gameObject.AddComponent<ObjectGenerator>();

        if(spawnCountPerObject != null && spawnCountPerObject.Length > 0 && environmentConfiguration.ComplexObjectConfig == ConfigType.High){
            objectGenerator.Initialize(layer, complexObjects, seed, spawnCountPerObject, gatherable);
        }
        else if(environmentConfiguration.ComplexObjectConfig == ConfigType.Low) {
            objectGenerator.Initialize(layer, objects, seed, objectAmount, gatherable);
        }
        else
        {
            objectGenerator.Initialize(layer, complexObjects, seed, objectAmount, gatherable);
        }

        meshes = new Mesh[(size/200)*(size/200)];
    }

    public void createNewEnvironment()
    {
        Initialize();

        for (int i = 0, x = 0; x < size; x += 200)
        {
            for (int z = 0; z < size; z += 200)
            {
                meshes[i++] = meshGenerator.CreateNewMesh(xMin+x, zMin+z, meshTag);
            }
        }

        CreateBorders();

        pathGenerator.GenerateLandmarkCoords(meshes, xMin + size / 2, zMin + size / 2);
        pathGenerator.GeneratePaths(meshes);
        pathGenerator.GenerateLandmarks(meshes);


        foreach(Mesh mesh in meshes)
        {
            if(spawnCountPerObject != null && spawnCountPerObject.Length > 0) {
                objectGenerator.GenerateObjectsWithSpawnCount(mesh, pathGenerator.getSpawn());
                Debug.Log("SpawnCount per Object");
            }
            else {
                objectGenerator.GenerateObjects(mesh, pathGenerator.getSpawn());
            }
        }
        
        if (generateGatherables)
        {
            objectGenerator.GenerateGatherables(pathGenerator.getPaths());
        }

        PlaySound();
    }

    private void PlaySound()
    {
        switch (gameObject.tag)
        {
            case "Environment1":
                AudioSrc.PlayOneShot(Environment1);
                break;
            case "Environment2":
                AudioSrc.PlayOneShot(Environment2);
                break;
            case "Environment3":
                AudioSrc.PlayOneShot(Environment3);
                break;
            case "Environment4":
                AudioSrc.PlayOneShot(Environment4);
                break;
            default:
                break;
        }
    }

    public Vector3 getSpawnPoint()
    {
        return pathGenerator.getSpawn();
    }

    public void ToggleGatherables()
    {
        generateGatherables = true;
    }

    private void CreateBorders()
    {
        int offSetFromOutside = 20;
        int halfOffSetFromOutside = offSetFromOutside / 2;

        // Create width borders
        float xOffset = size / 2;
        float zOffset = size;
        for (int i = 0; i < 2; i++)
        {
            GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);

            border.transform.localScale = new Vector3(size - offSetFromOutside, 100, 1);

            if (i == 0)
                border.transform.position = new Vector3(xMin + xOffset, 0, zMin + halfOffSetFromOutside);
            if (i == 1)
                border.transform.position = new Vector3(xMin + xOffset, 0, zMin + zOffset - halfOffSetFromOutside);

            border.GetComponent<Renderer>().material = wallMaterial;
            border.transform.name = "Width border " + (i + 1);
            border.transform.parent = transform;
        }

        // Create length borders
        xOffset = size;
        zOffset = size / 2;

        for (int i = 0; i < 2; i++)
        {
            GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);

            border.transform.localScale = new Vector3(1, 100, size - offSetFromOutside);

            if (i == 0)
                border.transform.position = new Vector3(xMin + halfOffSetFromOutside, 0, zMin + zOffset);
            if (i == 1)
                border.transform.position = new Vector3(xMin + xOffset - halfOffSetFromOutside, 0, zMin + zOffset);

            border.GetComponent<Renderer>().material = wallMaterial;
            border.transform.name = "Length border " + (i + 1);
            border.transform.parent = transform;
        }
    }
}
