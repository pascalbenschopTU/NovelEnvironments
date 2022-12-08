using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingHallGenerator : MonoBehaviour
{
    public int environment;

    private GameObject chosenEnvironment;

    private GameObject environment1;
    private GameObject environment2;
    private GameObject environment3;
    private GameObject environment4;

    private GameObject player;

    private Vector3 startingPosition;
   

    // Start is called before the first frame update
    void Start()
    {
        InitializeEnvironments();
        InitializePlayer();

        selectNextEnvironment();
        getStartingPosition();

        CreateHall();
    }

    private void InitializeEnvironments()
    {
        environment1 = GameObject.Find("Environment1");
        environment2 = GameObject.Find("Environment2");
        environment3 = GameObject.Find("Environment3");
        environment4 = GameObject.Find("Environment4");
    }

    private void InitializePlayer()
    {
        player = GameObject.Find("Player");
    }

    private void selectNextEnvironment()
    {
        switch(environment)
        {
            case 1:
                chosenEnvironment = environment1;
                break;
            case 2:
                chosenEnvironment = environment2;
                break;
            case 3:
                chosenEnvironment = environment3;
                break;
            case 4:
                chosenEnvironment = environment4;
                break;
            default:
                chosenEnvironment = environment1;
                break;
        }
    }

    private void getStartingPosition()
    {
        EnvironmentGenerator script = chosenEnvironment.GetComponent<EnvironmentGenerator>();

        startingPosition = new Vector3(script.xMin + 200, 50, script.yMin + 200);
    }

    private void CreateHall()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.localScale = new Vector3(2, 0, 2);
        floor.transform.position = startingPosition;

        player.transform.position = startingPosition + new Vector3(0, 1, 0);
    }
}
