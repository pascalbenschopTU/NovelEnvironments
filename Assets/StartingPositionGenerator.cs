using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingPositionGenerator : MonoBehaviour
{
    public EnvironmentConfiguration environmentConfiguration;

    private GameObject chosenEnvironment;

    private GameObject[] environments;

    private GameObject player;

    private Vector3 startingPosition;

    private EnvironmentGenerator script;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (ExperimentMetaData.Index >= ExperimentMetaData.Environments.Count)
        {
            Debug.Log("Experiment finished");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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

        ExperimentMetaData.Index += 1;
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

        startingPosition = script.getMeshStartingVertex() + new Vector3(0.0f, 1.0f, 0.0f);

        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.transform.SetPositionAndRotation(startingPosition, Quaternion.identity);
        cc.enabled = true;
    }

    private void StartTimer()
    {
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        yield return new WaitForSeconds(ExperimentMetaData.TimeInEnvironment);
        Debug.Log("Time has run out!");
        SceneManager.LoadScene("DefaultScene");
    }
}
