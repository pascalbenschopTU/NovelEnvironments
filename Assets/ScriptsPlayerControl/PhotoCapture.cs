using System.Collections;
using UnityEngine;
//using UnityEngine.Windows;
using UnityEngine.UI;
using System.IO;
using System;

public class PhotoCapture : MonoBehaviour
{
    private EnvironmentConfiguration environmentConfiguration;

    private AudioSource soundFX;

    public static bool isActive = true;

    private Texture2D screenCapture;

    private GameObject player;
    private Transform canvas;

    private GameObject cameraFlash;
    private float flashTime = 0.2f;

    private void Start()
    {
        screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        environmentConfiguration = ExperimentMetaData.currentEnvironment;
        player = gameObject;
        soundFX = gameObject.GetComponent<AudioSource>();

        CreateCameraFlash();
        AddOverLay();
    }

    private void CreateCameraFlash()
    {
        cameraFlash = new GameObject("LightFlash");
        Light lightComponent = cameraFlash.AddComponent<Light>();
        lightComponent.color = Color.white;
        lightComponent.range = 100;
        lightComponent.intensity = 1000;

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
                    sprite.GetComponent<Image>().color = Color.black;
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
                    sprite.GetComponent<Image>().color = Color.black;
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
        if (UnityEngine.Input.GetMouseButtonDown(0) && isActive)
        {
            AudioClip shutterSound = (AudioClip)Resources.Load("Sounds/camera-shutter-click", typeof(AudioClip));
            soundFX.PlayOneShot(shutterSound);
            StartCoroutine(CapturePhoto());
        }
    }

    IEnumerator CapturePhoto()
    {
        canvas.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        Rect regionToRead = new Rect(0, 0, Screen.width, Screen.height);

        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();

        cameraFlash.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        cameraFlash.SetActive(false);

        SavePhoto();
        canvas.gameObject.SetActive(true);
    }


    private void SavePhoto()
    {
        byte[] bytes = screenCapture.EncodeToPNG();
        var dirPath = Path.Join(ExperimentMetaData.LogDirectory, "pictures");
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        var path = Path.Join(dirPath , $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
        File.WriteAllBytes(path, bytes);

        LogPicture();
    }

    private void LogPicture()
    {
        TaskData task = new TaskData(
            new PositionalData(
                ExperimentMetaData.Index,
                DateTime.Now,
                player.transform.position, 
                player.transform.rotation
            ), 
            "Picture" // name of task
        );
        Recorder.RecordTaskData(task);
    }
}
