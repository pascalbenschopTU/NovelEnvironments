using UnityEngine;

public class AreaTracker : MonoBehaviour
{
	private string target = "Player";
	private Transform player;
	private Transform playerCamera;

	
    // Start is called before the first frame update
    void Start()
    {
		player = GameObject.Find(target).transform;
        playerCamera = player.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - player.position).sqrMagnitude < 250.0f)
        {
            Vector3 directionToMe = transform.position - playerCamera.position;
            if (Vector3.Angle(playerCamera.forward, directionToMe) < 45.0f)
            {
                LogSeenLandmark();
            }
        }
    }

    private void LogSeenLandmark()
    {
        TaskData task = new TaskData(
            new PositionalData(
                ExperimentMetaData.currentEnvironment.GetEnvironmentType(),
                System.DateTime.Now,
                transform.position,
                transform.rotation
            ),
            "Seen Landmark " + transform.name.Replace("(Clone)", string.Empty) // name of landmark
        );
        Recorder.RecordTaskData(task);
    }
}