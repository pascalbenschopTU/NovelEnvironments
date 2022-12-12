using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingPositionGenerator : MonoBehaviour
{
    public int environment;
    public int time;

    private GameObject chosenEnvironment;

    private GameObject[] environments;

    private GameObject player;

    private Vector3 startingPosition;


    // Start is called before the first frame update
    void Start()
    {
        environment = Settings.environment;
        
        InitializeEnvironments();
        InitializePlayer();

        if (environment != 0)
        {
            selectNextEnvironment();
            getStartingPosition();

            TeleportPlayer();
            StartTimer();
        }
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
        chosenEnvironment = environments[environment - 1];
        
        for (int i = 1; i <= environments.Length; i++)
        {
            if (i != environment)
            {
                EnvironmentGenerator script = environments[i - 1].GetComponent<EnvironmentGenerator>();
                script.layer = "Invisible";
            }
        }
    }

    private void getStartingPosition()
    {
        EnvironmentGenerator script = chosenEnvironment.GetComponent<EnvironmentGenerator>();

        startingPosition = new Vector3(script.xMin + 200, 50, script.zMin + 200);
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
