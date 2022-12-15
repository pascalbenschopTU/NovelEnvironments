using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingPositionGenerator : MonoBehaviour
{
    public EnvironmentConfiguration environmentConfiguration;
    public int time;

    private GameObject chosenEnvironment;

    private GameObject[] environments;

    private GameObject player;

    private Vector3 startingPosition;

    private EnvironmentGenerator script;


    // Start is called before the first frame update
    void Start()
    {
        if (Settings.index >= Settings.environments.Count)
        {
            Debug.Log("Experiment finished");
            SceneManager.LoadScene("MainMenu");
            return;
        }
        environmentConfiguration = Settings.environments[Settings.index];

        if (environmentConfiguration == null)
        {
            Debug.LogError("Synchronization error, environment not correctly configured.");
            return;
        }

        InitializeEnvironments();
        InitializePlayer();

        selectNextEnvironment();
        getStartingPosition();

        TeleportPlayer();
        StartTimer();

        Settings.index += 1;
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
    }

    private void setPlayerMiniMap()
    {
        if (player.transform.Find("Canvas") != null)
        {
            GameObject canvas = player.transform.Find("Canvas").gameObject;
            if (environmentConfiguration.MapConfig == ConfigType.Low)
            {
                canvas.SetActive(false);
            }
            else
            {
                canvas.SetActive(true);
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

    private void selectNextEnvironment()
    {
        chosenEnvironment = environments[(int)environmentConfiguration.EnvironmentType];

        script = chosenEnvironment.GetComponent<EnvironmentGenerator>();
        script.objectAmount = environmentConfiguration.GetNumberObjectsConfigValue();
        script.createNewEnvironment();
    }

    private void getStartingPosition()
    {
        startingPosition = script.getMeshStartingVertex() + new Vector3(0.0f, 1.0f, 0.0f);
    }

    private void TeleportPlayer()
    {
        player.transform.position = startingPosition;
    }

    private void StartTimer()
    {
        StartCoroutine(countDown());
    }

    private IEnumerator countDown()
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Time has run out!");
        SceneManager.LoadScene("DefaultScene");
    }
}
