using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour
{
    private MeshGenerator meshGenerator;
    private PathGenerator pathGenerator;
    private ObjectGenerator objectGenerator;

    public string layer = "Ground";

    public GameObject[] objects;
    public GameObject[] landmarks;

    public string meshTag;

    public int objectAmount;

    public Material terrainMaterial;

    [SerializeField] private AnimationCurve heightCurve;

    public int xMin;
    public int zMin;

    public float scale;
    public int octaves;
    public float lacunarity;
 
    public int seed;

    public Gradient gradient;

    public int size = 400;
    

    private Mesh[] meshes;
    private int index = 0;

    [SerializeField] private AudioSource AudioSrc = default;
    [SerializeField] private AudioClip Environment1 = default;
    [SerializeField] private AudioClip Environment2 = default;
    [SerializeField] private AudioClip Environment3 = default;
    [SerializeField] private AudioClip Environment4 = default;

    public void Initialize()
    {

        meshGenerator = gameObject.AddComponent<MeshGenerator>();
        meshGenerator.Initialize(layer, objects, landmarks, terrainMaterial, heightCurve, scale, octaves, lacunarity, seed, gradient);
        pathGenerator = gameObject.AddComponent<PathGenerator>();
        pathGenerator.Initialize(layer, landmarks, seed, terrainMaterial);
        objectGenerator = gameObject.AddComponent<ObjectGenerator>();
        objectGenerator.Initialize(layer, objects, seed, objectAmount);
        Debug.Log(gameObject.tag);

        switch(gameObject.tag)
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
                    // default:
                    //     AudioSrc.PlayOneShot(Environment4);
                    //     break;

                }


        meshes = new Mesh[4];
    }

    public void createNewEnvironment()
    {
        Initialize();

        meshes[index++] = meshGenerator.CreateNewMesh(xMin, zMin, meshTag);
        meshes[index++] = meshGenerator.CreateNewMesh(xMin, zMin + size / 2, meshTag);
        meshes[index++] = meshGenerator.CreateNewMesh(xMin + size / 2, zMin, meshTag);
        meshes[index++] = meshGenerator.CreateNewMesh(xMin + size / 2, zMin + size / 2, meshTag);

        createBorders();

        pathGenerator.GenerateLandmarkCoords(meshes, xMin + size / 2, zMin + size / 2);
        pathGenerator.GeneratePaths(meshes);
        pathGenerator.GenerateLandmarks(meshes);

        GameObject temp = new GameObject("EnvironmentObjects");
        temp.transform.parent = transform;

        foreach(Mesh mesh in meshes)
        {
            objectGenerator.GenerateObjects(mesh, temp);
        }
    }

    public Vector3 getMeshStartingVertex()
    {
        Mesh mesh = meshes[3];
        Vector3 vector3 = mesh.vertices[0];

        return vector3;
    }

    private void createBorders()
    {
        int offSetFromOutside = 30;
        int halfOffSetFromOutside = offSetFromOutside / 2;

        // Create width borders
        float xOffset = size / 2;
        float zOffset = size;
        for (int i = 0; i < 2; i++)
        {
            GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);

            border.transform.localScale = new Vector3(size - offSetFromOutside, size - offSetFromOutside, 1);

            if (i == 0)
                border.transform.position = new Vector3(xMin + xOffset, 0, zMin + halfOffSetFromOutside);
            if (i == 1)
                border.transform.position = new Vector3(xMin + xOffset, 0, zMin + zOffset - halfOffSetFromOutside);

            border.GetComponent<MeshRenderer>().enabled = false;
            border.transform.name = "Width border " + (i + 1);
            border.transform.parent = transform;
        }

        // Create length borders
        xOffset = size;
        zOffset = size / 2;

        for (int i = 0; i < 2; i++)
        {
            GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);

            border.transform.localScale = new Vector3(1, size - offSetFromOutside, size - offSetFromOutside);

            if (i == 0)
                border.transform.position = new Vector3(xMin + halfOffSetFromOutside, 0, zMin + zOffset);
            if (i == 1)
                border.transform.position = new Vector3(xMin + xOffset - halfOffSetFromOutside, 0, zMin + zOffset);

            border.GetComponent<MeshRenderer>().enabled = false;
            border.transform.name = "Length border " + (i + 1);
            border.transform.parent = transform;
        }
    }
}
