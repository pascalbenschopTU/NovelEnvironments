using System.Collections;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public float range = 10.0f;
    public Camera fpsCam;

    private void Start()
    {
        GameObject camera = gameObject.transform.Find("Main Camera").gameObject;
        fpsCam = camera.GetComponent<Camera>();
    }

    void Update()
    {
        LayerMask layer = LayerMask.NameToLayer("Forageable");
        RaycastHit[] hits;
        hits = Physics.RaycastAll(fpsCam.transform.position, fpsCam.transform.forward, range);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.transform.gameObject.layer == layer)
            {
                foreach (Transform trans in hit.transform.gameObject.GetComponentsInChildren<Transform>(true))
                {
                    if (trans.gameObject.GetComponent<MeshRenderer>() != null)
                    {
                        StartCoroutine(ChangeColor(trans.gameObject.GetComponent<MeshRenderer>()));
                    }
                }
            }
        }
    }

    IEnumerator ChangeColor(MeshRenderer renderer)
    {
        Color c = renderer.material.color;
        Color temp = new Color(c.r, c.g, c.b, c.a);
        renderer.material.color = new Color(255, 255, 255);
        yield return new WaitForSeconds(1.0f);
        renderer.material.color = temp;
    }
}
