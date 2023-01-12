using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementReplayController : MonoBehaviour
{
    public int environment_id;

    private List<PositionalData> replayData { get; set; }
    private float recordingInterval = 0.025f;
    private float movementStep = 12.0f * 0.025f; // speed * recordingInterval
    private float rotationStep = 1.0f;
    private int index = 0;
    
    // Use this for initialization
    void Start()
    {
        // Get replay data from csv stored in folder ReplayData
        environment_id = (int)ExperimentMetaData.currentEnvironment.EnvironmentType;
        Dictionary<int, List<PositionalData>> allReplayData = CsvUtils.PositionalReplayDataFromCsv();
        replayData = allReplayData[environment_id];

        DisableMovement();

        InvokeRepeating("MovePlayer", 0f, recordingInterval);
    }

    private void DisableMovement()
    {
        // Disable character controller
        CharacterController cc = transform.GetComponent<CharacterController>();
        cc.enabled = false;
        // Disable player movement
        PlayerMovement playerMovementScript = transform.GetComponent<PlayerMovement>();
        playerMovementScript.enabled = false;
    }

    private void MovePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, replayData[index].position, movementStep);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, replayData[index].rotation, rotationStep);
        index++;
    }
}
