using System.Collections;
using UnityEngine;
//using UnityEngine.Windows;
using UnityEngine.UI;
using System.IO;

public class PhotoCapture : MonoBehaviour
{
    private Texture2D screenCapture;

    private GameObject player;
    private Transform canvas;

    private GameObject cameraFlash;
    private float flashTime = 0.2f;

    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        player = gameObject;

        CreateCameraFlash();
        AddOverLay();
    }

    private void CreateCameraFlash()
    {
        cameraFlash = new GameObject("LightFlash");
        Light lightComponent = cameraFlash.AddComponent<Light>();
        lightComponent.color = Color.white;
        lightComponent.range = 100;
        lightComponent.intensity = 10;

        cameraFlash.transform.parent = player.transform;
        cameraFlash.transform.position = player.transform.position + player.transform.forward;

        cameraFlash.SetActive(false);
    }

    private void AddOverLay()
    {
        canvas = player.transform.Find("Canvas");
        if (canvas != null)
        {
            GameObject panel = new GameObject("Panel");

            RectTransform rectTransform = panel.AddComponent<RectTransform>();
            rectTransform.transform.SetParent(canvas, false);
            rectTransform.anchoredPosition = new Vector2(0f, 0f);
            rectTransform.sizeDelta = new Vector2(100f, 100f);

            // Top and bottom
            for (int i = 0; i < 2; i++)
            {
                // Left and right
                for (int j = 0; j < 2; j++)
                {
                    GameObject sprite = new GameObject("Sprite " + i + j);
                    sprite.AddComponent<Image>();
                    RectTransform spriteTransform = sprite.GetComponent<RectTransform>();
                    spriteTransform.transform.SetParent(panel.transform, false);
                    spriteTransform.anchorMin = new Vector2(Mathf.Abs(j - 0.1f), i);
                    spriteTransform.anchorMax = new Vector2(Mathf.Abs(j - 0.1f), i);
                    spriteTransform.sizeDelta = new Vector2(25f, 5f);
                }
            }

            // Left and right
            for (int i = 0; i < 2; i++)
            {
                // Top and bottom
                for (int j = 0; j < 2; j++)
                {
                    GameObject sprite = new GameObject("Sprite " + (i + 2) + (j+2));
                    sprite.AddComponent<Image>();
                    RectTransform spriteTransform = sprite.GetComponent<RectTransform>();
                    spriteTransform.transform.SetParent(panel.transform, false);
                    spriteTransform.anchorMin = new Vector2(i, Mathf.Abs(j - 0.1f));
                    spriteTransform.anchorMax = new Vector2(i, Mathf.Abs(j - 0.1f));
                    spriteTransform.sizeDelta = new Vector2(5f, 25f);
                } 
            }
        }
    }

    private void Update()
    {
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            StartCoroutine(CapturePhoto());
        }
    }

    IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);

        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();

        cameraFlash.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        cameraFlash.SetActive(false);

        SavePhoto();
    }


    private void SavePhoto()
    {
        byte[] bytes = screenCapture.EncodeToPNG();
        var dirPath = Application.dataPath + "/Pictures/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        string path = dirPath + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        File.WriteAllBytes(path, bytes);

        storePictureInDB(path);
    }

    private void storePictureInDB(string filepath) {
        int participant_id = ExperimentMetaData.ParticipantNumber;
        
        int index = ExperimentMetaData.Index;

        EnvironmentConfiguration environmentConfiguration = null;

        if(index > 0) {
            environmentConfiguration = ExperimentMetaData.Environments[index-1];
        }
        else {
            return;
        }

        if(environmentConfiguration != null) 
        {
            int environment_id = (int)ExperimentMetaData.Environments[index-1].EnvironmentType;
            player.GetComponent<SqliteLogging>().storePicture(participant_id, environment_id, filepath);
        }
        else 
        {
            Debug.Log("Cannot Take Picture rn");
            return;
        }
    }
}
