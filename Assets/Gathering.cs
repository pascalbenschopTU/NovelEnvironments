using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Gathering : MonoBehaviour
{
    private EnvironmentConfiguration environmentConfiguration;

    private enum UpDown { Down = -1, Start = 0, Up = 1 };
    private Text text;
    private Text text2;
    string baseText2 = "Lanterns ";
    string baseText = "Press E to Collect Lantern ";
    string shownText;
    float raycastDistance = 5;
    int itemsToCollect = 3;
    int itemsCollected = 0;

    void Start()
    {
        environmentConfiguration = ExperimentMetaData.currentEnvironment;
        shownText = "Press E to Collect Lantern 0/" + itemsToCollect.ToString();
        // Load the Arial font from the Unity Resources folder.
        Font arial;
        arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        // Create Canvas GameObject.
        GameObject canvasGO = new GameObject();
        canvasGO.name = "Canvas";
        canvasGO.AddComponent<Canvas>();
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Get canvas from the GameObject.
        Canvas canvas;
        canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Create the Text GameObject.
        GameObject textGO = new GameObject();
        textGO.transform.parent = canvasGO.transform;
        textGO.AddComponent<Text>();

        // Set Text component properties.
        text = textGO.GetComponent<Text>();
        text.font = arial;
        text.text = "";
        text.fontSize = 48;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;

        // Provide Text position and size using RectTransform.
        RectTransform rectTransform;
        rectTransform = text.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 0, 0);
        rectTransform.sizeDelta = new Vector2(600, 200);

        // Create the Text GameObject.
        GameObject text2GO = new GameObject();
        text2GO.transform.parent = canvasGO.transform;
        text2GO.AddComponent<Text>();

        // Set Text component properties.
        text2 = text2GO.GetComponent<Text>();
        text2.font = arial;
        text2.text = "Lanterns 0/" + itemsToCollect.ToString();
        text2.fontSize = 48;
        text2.color = Color.black;
        text2.alignment = TextAnchor.UpperLeft;

        // Provide Text position and size using RectTransform.
        RectTransform rectTransform2;
        rectTransform2 = text2.GetComponent<RectTransform>();
        //rectTransform2.localPosition = new Vector3(0, 0, 0);
        rectTransform2.sizeDelta = new Vector2(600, 200);
        rectTransform2.anchoredPosition = new Vector2(-600, 350);
        System.Console.WriteLine("anchoredPosition: {0}", rectTransform2.anchoredPosition);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Gather")) {
                if (itemsCollected == itemsToCollect) {
                    shownText = "All Lanterns Collected";
                    text.color = Color.magenta;
                } else if (Input.GetKeyDown(KeyCode.E)) {
                    itemsCollected++;
                    shownText = baseText + itemsCollected.ToString() + "/" + itemsToCollect.ToString();
                    text2.text = baseText2 + itemsCollected.ToString() + "/" + itemsToCollect.ToString();
                    Destroy(hit.collider.gameObject);
                    LogGathering();
                }
                text.text = shownText;
                return;
            }
        }

        text.text = "";
    }

    private void LogGathering()
    {
        TaskData task = new TaskData(
            new PositionalData(
                (int)environmentConfiguration.EnvironmentType,
                gameObject.transform.position,
                gameObject.transform.rotation
            ),
            "Gathering" // name of task
        );
        Recorder.RecordTaskData(task);
    }
}
