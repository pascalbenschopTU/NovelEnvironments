using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LookDirection : MonoBehaviour
{
    private EnvironmentConfiguration environmentConfiguration;

    public GameObject playerCamera;
    public GameObject target;

    private void Start()
    {
        environmentConfiguration = ExperimentMetaData.currentEnvironment;
    }

    // Update is called once per frame
    void Update()
    {
        CheckDirection(target, playerCamera);
    }

    bool inArea = false;

    public void CheckDirection(GameObject _target, GameObject camera) {
        if (_target== false) {
            return;
        }

        Vector3 directionToMe = _target.transform.position - camera.transform.position;
        if (Vector3.Angle(camera.transform.forward, directionToMe) < 45.0f)
        {
            LogSeenLandmark(_target);
        }
    }

    private void LogSeenLandmark(GameObject target)
    {
        TaskData task = new TaskData(
            new PositionalData(
                ExperimentMetaData.Index,
                target.transform.position,
                target.transform.rotation
            ),
            "Landmark" // name of task
        );
        Recorder.RecordTaskData(task);
    }

    public void setTarget(GameObject tar) {
        target = tar;
    }

    public void SetInArea(bool _inArea) {
        inArea = _inArea;
    }
}
