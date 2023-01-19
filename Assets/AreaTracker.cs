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
        if ((transform.position - player.position).sqrMagnitude < 100.0f)
		{
            Vector3 directionToMe = transform.position - playerCamera.position;
            if (Vector3.Angle(playerCamera.forward, directionToMe) < 45.0f)
			{
                Debug.Log("Spotted a landmark");
                LogSeenLandmark();
            }
        }
    }

    private void LogSeenLandmark()
    {
        TaskData task = new TaskData(
            new PositionalData(
                ExperimentMetaData.Index,
                System.DateTime.Now,
                transform.position,
                transform.rotation
            ),
            "Seen Landmark " + transform.name // name of task
        );
        Recorder.RecordTaskData(task);
    }
}