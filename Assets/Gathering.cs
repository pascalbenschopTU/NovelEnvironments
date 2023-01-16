using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Gathering : MonoBehaviour
{
    private EnvironmentConfiguration environmentConfiguration;

    private enum UpDown { Down = -1, Start = 0, Up = 1 };
    private Text text;
    string shownText = "Press E to collect lantern";
    float raycastDistance = 5;

    void Start()
    {
        environmentConfiguration = ExperimentMetaData.currentEnvironment;
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
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Gather")) {
                if (Input.GetKeyDown(KeyCode.E)){
                    shownText = "Lantern collected";
                    text.color = Color.magenta;
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
                ExperimentMetaData.Index,
                gameObject.transform.position,
                gameObject.transform.rotation
            ),
            "Gathering" // name of task
        );
        Recorder.RecordTaskData(task);
    }
}
