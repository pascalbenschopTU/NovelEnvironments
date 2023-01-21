﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.IO;

public class StartingPositionGenerator : MonoBehaviour
{
    public EnvironmentConfiguration environmentConfiguration;

    [SerializeField] private LoadingScreenManager LoadingScreenManager;
    private GameObject chosenEnvironment;

    private GameObject[] environments;

    private GameObject player;

    private Vector3 startingPosition;

    private EnvironmentGenerator script;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (ExperimentMetaData.Index >= ExperimentMetaData.Environments.Count)
        // if (ExperimentMetaData.Index >= 0)
        {
            Debug.Log("Experiment finished");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ExperimentMetaData.ExperimentFinished = true;
            LoadingScreenManager.LoadScene("MainMenu");
            return;
        }

        environmentConfiguration = ExperimentMetaData.Environments[ExperimentMetaData.Index];

        if (environmentConfiguration == null)
        {
            Debug.LogError("Synchronization error, environment not correctly configured.");
            return;
        }

        ExperimentMetaData.currentEnvironment = environmentConfiguration;

        InitializePlayer();
        InitializeEnvironments();
        SelectNextEnvironment();
        StartTimer();
    }

    private void InitializeEnvironments()
    {
        environments = new GameObject[4];

        environments[0] = GameObject.Find("Environment1");
        environments[1] = GameObject.Find("Environment2");
        environments[2] = GameObject.Find("Environment3");
        environments[3] = GameObject.Find("Environment4");
    }

    private void InitializePlayer()
    {
        player = GameObject.Find("Player");
        setPlayerMiniMap();
        setPlayerFOV();
        TogglePlayerCamera();
        ToggleLowInvolvement();
    }

    private void setPlayerMiniMap()
    {
        Transform canvas = player.transform.Find("Canvas");
        if (canvas != null)
        {
            GameObject minimap = canvas.Find("RawImage").gameObject;
            if (environmentConfiguration.MapConfig == ConfigType.Low)
            {
                minimap.SetActive(false);
            }
            else
            {
                minimap.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("No canvas attached to player?");
        }
    }

    private void setPlayerFOV()
    {
        if (player.transform.Find("Main Camera") != null)
        {
            GameObject camera = player.transform.Find("Main Camera").gameObject;
            Camera c = camera.GetComponent<Camera>();
            c.farClipPlane = environmentConfiguration.GetRDConfigValue();
        }
        else
        {
            Debug.LogWarning("No camera attached to player?");
        }
    }

    private void TogglePlayerCamera()
    {
        PhotoCapture photoCaptureScript = player.GetComponent<PhotoCapture>();

        if (photoCaptureScript != null)
        {
            if (environmentConfiguration.CameraTask)
            {
                photoCaptureScript.enabled = true;
            } 
            else
            {
                photoCaptureScript.enabled = false;
            }
        }
    }


    private void ToggleLowInvolvement()
    {
        if (environmentConfiguration.InteractionConfig == ConfigType.Low)
        {
            player.AddComponent<PlayerMovementReplayController>();
        }
    }

    private void SelectNextEnvironment()
    {
        chosenEnvironment = environments[(int)environmentConfiguration.EnvironmentType];

        script = chosenEnvironment.GetComponent<EnvironmentGenerator>();
        script.objectAmount = environmentConfiguration.GetComplexObjectsConfigValue();
        ToggleGathering();
        script.createNewEnvironment();

        startingPosition = script.getSpawnPoint();

        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.transform.SetPositionAndRotation(startingPosition, Quaternion.identity);
        cc.enabled = true;
    }

    private void ToggleGathering()
    {
        if (environmentConfiguration.PickupTask)
        {
            player.AddComponent<Gathering>();
            script.ToggleGatherables();
        }
    }

    private void StartTimer()
    {
        Recorder.StartRecording();
        GameTime.RestartGameTime();
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        yield return new WaitForSeconds(ExperimentMetaData.TimeInEnvironment);
        Debug.Log("Time has run out!");

        Recorder.StopRecording();
        GameTime.AddGameTime();

        ExperimentMetaData.Index++;

        LoadingScreenManager.LoadSceneWait("DefaultScene", 1.5f);
    }

}
