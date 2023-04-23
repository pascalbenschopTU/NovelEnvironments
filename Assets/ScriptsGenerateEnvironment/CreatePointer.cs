using UnityEngine;

public class CreatePointer : MonoBehaviour
{
    private GameObject player;

    public Material material;

    // Start is called before the first frame update
    public void AddPointer()
    {
        player = transform.parent.gameObject;
        AddPointerToPlayer(5, 31, Color.white);
        AddPointerToPlayer(8, 30, Color.black);
    }

    private void AddPointerToPlayer(int size, int height, Color color)
    {
        Mesh mesh = new Mesh();

        Vector3[] VerticesArray = new Vector3[3];
        int[] trianglesArray = new int[3];

        VerticesArray[0] = player.transform.position + new Vector3(-size, height, -size);
        VerticesArray[2] = player.transform.position + new Vector3(size, height, -size);
        VerticesArray[1] = player.transform.position + new Vector3(0, height, size + 1);

        trianglesArray[0] = 0;
        trianglesArray[1] = 1;
        trianglesArray[2] = 2;

        mesh.vertices = VerticesArray;
        mesh.triangles = trianglesArray;

        AddMeshToObject(mesh, color);
    }

    private void AddMeshToObject(Mesh mesh, Color color)
    {
        GameObject go = new GameObject("Pointer");
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshCollider>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<MeshRenderer>().material = material;
        go.GetComponent<MeshRenderer>().material.color = color;
        go.GetComponent<MeshFilter>().sharedMesh = mesh;
        go.GetComponent<MeshCollider>().sharedMesh = mesh;
        go.GetComponent<MeshCollider>().enabled = true;

        go.layer = LayerMask.NameToLayer("PlayerPointer");

        go.transform.parent = player.transform;
    }
}
