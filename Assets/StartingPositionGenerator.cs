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
    }

    private void selectNextEnvironment()
    {
        chosenEnvironment = environments[(int)environmentConfiguration.EnvironmentType];

        script = chosenEnvironment.GetComponent<EnvironmentGenerator>();
        script.createNewEnvironment();
    }

    private void getStartingPosition()
    {
        startingPosition = script.getMeshStartingVertex() + new Vector3(0.0f, 0.5f, 0.0f);
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
