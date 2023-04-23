using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.IO;

public class StartingPositionGenerator : MonoBehaviour
{
    public EnvironmentConfiguration environmentConfiguration;

    [SerializeField] private LoadingScreenManager LoadingScreenManager;
    private EnvironmentSetup chosenEnvironment;

    private EnvironmentSetup[] environments;
    private AudioClip[] sounds;

    private GameObject player;
    private GameObject startingHall;

    private Vector3 startingPosition;

    private EnvironmentGenerator script;

    private int StartingHallHeight = 202;

    // Start is called before the first frame update
    void OnEnable()
    {
        startingHall = GameObject.Find("StartingHall");
        InitializeEnvironment();
    }

    // Updates the environment with the next one or finishes the game.
    public void InitializeEnvironment()
    {
        if (ExperimentMetaData.Index >= ExperimentMetaData.Environments.Count)
        {
            Debug.Log("Experiment finished");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ExperimentMetaData.ExperimentFinished = true;
            LoadingScreenManager.LoadScene("MainMenu");
            return;
        }

        environmentConfiguration = ExperimentMetaData.Environments[ExperimentMetaData.Index];

        if (environmentConfiguration == null)
        {
            Debug.LogError("Synchronization error, environment not correctly configured.");
            return;
        }

        StartCoroutine(LoadEnvironment());
    }

    // This is to make sure that the loading screen is shown before it starts to load the environment
    IEnumerator LoadEnvironment()
    {
        yield return new WaitUntil(() => LoadingScreenManager.Loading());

        ExperimentMetaData.currentEnvironment = environmentConfiguration;
        InitializePlayer();
        InitializeEnvironments();
        SelectNextEnvironment();
        LoadingScreenManager.StopLoading();
        // Make sure player at correct position
        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.transform.SetPositionAndRotation(new Vector3(0, StartingHallHeight, 0), Quaternion.Euler(new Vector3(0, 180, 0)));
        cc.enabled = true;
    }

    // Teleports player to environment and makes sure correct settings are applied
    public void TeleportPlayerToEnvironment()
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.transform.SetPositionAndRotation(startingPosition, Quaternion.identity);
        cc.enabled = true;

        startingHall.SetActive(false);

        SetPlayerMiniMap();
        ToggleLowInvolvement();
        StartTimer();
        PlayEnvironmentSound();
    }

    private void InitializeEnvironments()
    {
        environments = new EnvironmentSetup[4];

        environments[0] = Forest.GetEnvironmentSetup();
        environments[1] = Desert.GetEnvironmentSetup();
        environments[2] = City.GetEnvironmentSetup();
        environments[3] = Snow.GetEnvironmentSetup();
        
        sounds = new AudioClip[4];

        sounds[0] = (AudioClip)Resources.Load("Sounds/forest", typeof(AudioClip));
        sounds[1] = (AudioClip)Resources.Load("Sounds/desert", typeof(AudioClip));
        sounds[2] = (AudioClip)Resources.Load("Sounds/city", typeof(AudioClip));
        sounds[3] = (AudioClip)Resources.Load("Sounds/snow", typeof(AudioClip));
    }

    private void PlayEnvironmentSound()
    {
        player.GetComponent<AudioSource>().PlayOneShot(sounds[(int)environmentConfiguration.EnvironmentType]);
    }

    public void InitializePlayer()
    {
        player = GameObject.Find("Player");
        SetPlayerFOV();
        TogglePlayerCamera();
        player.transform.Find("Canvas").Find("RawImage").gameObject.SetActive(false);
    }

    private void SetPlayerMiniMap()
    {
        Transform canvas = player.transform.Find("Canvas");
        if (canvas != null)
        {
            GameObject minimap = canvas.Find("RawImage").gameObject;
            Transform minimapCamera = player.transform.Find("MiniMapCamera");
            if (minimapCamera != null)
            {
                minimapCamera.position = new Vector3(0, 450, 0);
                minimapCamera.GetComponent<CreatePointer>().AddPointer();
            }
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

    private void SetPlayerFOV()
    {
        if (player.transform.Find("Main Camera") != null)
        {
            GameObject camera = player.transform.Find("Main Camera").gameObject;
            Camera c = camera.GetComponent<Camera>();
            c.farClipPlane = environmentConfiguration.GetRDConfigValue();
        }
        else
        {
            Debug.LogWarning("No camera attached to player?");
        }
    }

    private void TogglePlayerCamera()
    {
        if (environmentConfiguration.CameraTask)
        {
            player.AddComponent<PhotoCapture>();
        } 
  
    }


    private void ToggleLowInvolvement()
    {
        if (environmentConfiguration.InteractionConfig == ConfigType.Low)
        {
            player.AddComponent<PlayerMovementReplayController>();
        }
    }

    private void SelectNextEnvironment()
    {
        chosenEnvironment = environments[(int)environmentConfiguration.EnvironmentType];

        script = gameObject.AddComponent<EnvironmentGenerator>();
        script.InitializeEnvironmentFromSetup(chosenEnvironment);

        ToggleGathering();
        script.CreateNewEnvironment();

        startingPosition = script.getSpawnPoint(); 
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
        Recorder.StartRecording();
        GameTime.RestartGameTime();
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        yield return new WaitForSeconds(ExperimentMetaData.TimeInEnvironment);
        Debug.Log("Time has run out!");

        Recorder.StopRecording();
        GameTime.AddGameTime();

        ExperimentMetaData.Index++;

        ResetEnvironment();

        LoadingScreenManager.Loading();
        InitializeEnvironment();
    }

    private void ResetEnvironment()
    {
        startingHall.SetActive(true);

        PurgePreviousEnvironment();
        ResetTextInstructions();

        player.GetComponent<AudioSource>().Stop();
        if (player.GetComponent<PlayerMovementReplayController>() != null)
        {
            Destroy(player.GetComponent<PlayerMovementReplayController>());
            EnableMovement();
        }
        if (player.GetComponent<Gathering>() != null)
        {
            Destroy(player.GetComponent<Gathering>());
        }
        if (player.GetComponent<PhotoCapture>() != null)
        {
            Destroy(player.GetComponent<PhotoCapture>());
        }
    }

    private void PurgePreviousEnvironment()
    {
        var allGameObjects = FindObjectsOfType<GameObject>();
        var gameObjectsToDestroy = new ArrayList();
        for (var i = 0; i < allGameObjects.Length; i++)
        {
            if (allGameObjects[i].name.Contains("Clone") 
                || allGameObjects[i].name == "Mesh"
                || allGameObjects[i].name == "PathMesh"
                || allGameObjects[i].name == "Cube"
                || allGameObjects[i].name == "Pointer"
                || allGameObjects[i].name.Contains("Sprite")
                || allGameObjects[i].name.Contains("Gather")
            )
            {
                gameObjectsToDestroy.Add(allGameObjects[i]);
            }
        }

        foreach (GameObject obj in gameObjectsToDestroy)
        {
            Destroy(obj);
        }
    }

    private void ResetTextInstructions()
    {
        var allGameObjects = FindObjectsOfType<GameObject>();
        for (var i = 0; i < allGameObjects.Length; i++)
        {
            if (allGameObjects[i].name.Contains("Text (TMP)"))
            {
                GameObject text = allGameObjects[i];
                if (text.GetComponent<SetText>() != null)
                {
                    // Refresh text
                    text.GetComponent<SetText>().enabled = false;
                    text.GetComponent<SetText>().enabled = true;
                }
            }
        }
    }

    private void EnableMovement()
    {
        // Disable character controller
        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = true;
        // Disable player movement
        PlayerMovement playerMovementScript = player.GetComponent<PlayerMovement>();
        playerMovementScript.enabled = true;
        // Disable mouse input
        Transform camera = player.transform.Find("Main Camera");
        camera.GetComponent<MouseLook>().enabled = true;
        
    }
}
