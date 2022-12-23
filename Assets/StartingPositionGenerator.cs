using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class StartingPositionGenerator : MonoBehaviour
{
    public EnvironmentConfiguration environmentConfiguration;

    private GameObject chosenEnvironment;

    private GameObject[] environments;

    private GameObject player;

    private Vector3 startingPosition;

    private EnvironmentGenerator script;

    UnityEvent endEnvironmentEvent = new UnityEvent();

    // Start is called before the first frame update
    void OnEnable()
    {
        if (ExperimentMetaData.Index >= ExperimentMetaData.Environments.Count)
        {
            Debug.Log("Experiment finished");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ExperimentMetaData.ExperimentFinished = true;
            SceneManager.LoadScene("MainMenu");
            return;
        }

        environmentConfiguration = ExperimentMetaData.Environments[ExperimentMetaData.Index];

        if (environmentConfiguration == null)
        {
            Debug.LogError("Synchronization error, environment not correctly configured.");
            return;
        }

        InitializePlayer();
        InitializeEnvironments();
        
        selectNextEnvironment();
        StartTimer();

        // endEnvironmentEvent.AddListener(player.GetComponent<Recorder>().storeRecording);
        ExperimentMetaData.Index += 1;

        storeParticipantAndEnvironment();
    }

    private void storeParticipantAndEnvironment() {
        int participant_id = ExperimentMetaData.ParticipantNumber;
        int environment_id = (int)environmentConfiguration.EnvironmentType;

        player.GetComponent<SqliteLogging>().createUserEnvironment(participant_id, environment_id);

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
            if (environmentConfiguration.FOVConfig == ConfigType.Low)
            {
                c.fieldOfView = environmentConfiguration.GetFOVConfigValue();
            }
            else
            {
                c.fieldOfView = environmentConfiguration.GetFOVConfigValue();
            }
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

    private void selectNextEnvironment()
    {
        chosenEnvironment = environments[(int)environmentConfiguration.EnvironmentType];

        script = chosenEnvironment.GetComponent<EnvironmentGenerator>();
        script.objectAmount = environmentConfiguration.GetNumberObjectsConfigValue();
        ToggleGathering();
        script.createNewEnvironment();

        startingPosition = script.getSpawnPoint();
        // startingPosition = script.getMeshStartingVertex() + new Vector3(0.0f, 1.0f, 0.0f);

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
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {

//        yield return new WaitForSeconds(Settings.time);
        yield return new WaitForSeconds(ExperimentMetaData.TimeInEnvironment);

        Debug.Log("Ending Environment");
        endEnvironmentEvent.Invoke();
        Queue<ReplayData> rq = player.GetComponent<Recorder>().recordingQueue;
        Debug.Log("Recording Queue Size: " + rq.Count);
        // player.GetComponent<SqliteLogging>().storeUserPositionQueue(11, 11, rq);
        // player.GetComponent<SqliteLogging>().getCountPictureByUserInEnvironment(66);

        Debug.Log("Time has run out!");
        SceneManager.LoadScene("DefaultScene");
    }

    public GameObject getChosenEnv() {
        return this.chosenEnvironment;
    }
}
