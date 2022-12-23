using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;
    public EnvironmentConfiguration environmentConfiguration;


    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f; 
    [SerializeField] private bool useFootsteps = true;
    [SerializeField] private AudioSource footStepsAudioSrc = default;
    [SerializeField] private AudioClip[] grassSounds = default;
    [SerializeField] private AudioClip[] concreteSounds = default;
    [SerializeField] private AudioClip[] snowSounds = default;
    [SerializeField] private AudioClip[] sandSounds = default;

    private float footStepTimer = 0;

    public Transform groundCheck;
    private float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;

    public Transform minimapCamera;
    public Camera playerCamera;
    private Vector2 currentInput;

    private Recorder recorder;

    private GameObject player;
    private Scene scene;
    private int recording_step;

    private void Awake()
    {
        int index = ExperimentMetaData.Index;
        if(index > 0) {
            environmentConfiguration = ExperimentMetaData.Environments[index-1];
        } else
        {
            environmentConfiguration = ExperimentMetaData.Environments[0];
        }
        recorder = GetComponent<Recorder>();
        recording_step = 0;
    }

    private void Start()
    {

        player = GameObject.Find("Player");

        // Log data every .5 seconds
        InvokeRepeating("logData", 0f, 0.5f);
        scene = SceneManager.GetActiveScene();
    }

    private void logData()
    {
        ReplayData data = new ReplayData(this.transform.position, this.transform.rotation);
        if((recorder != null) && (scene.name != "DefaultScene")) 
        {
            recorder.recordReplayFrame(data);

            int participant_id = ExperimentMetaData.ParticipantNumber;
            int environment_id = (int)environmentConfiguration.EnvironmentType;

            player.GetComponent<SqliteLogging>().storeUserPosition(participant_id, environment_id, data, recording_step);
            player.GetComponent<SqliteLogging>().storeUserRotation(participant_id, environment_id, data, recording_step);

            recording_step++;
        }
    }

    private void HandleFootSteps()
    {
        currentInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal") ); 

        if(!controller.isGrounded) return;
        if(currentInput == Vector2.zero) return;
        footStepTimer -= Time.deltaTime;

        RaycastHit hit;
        Ray landingRay = new Ray(transform.position, Vector3.down);


        if(footStepTimer <= 0) {
            if(Physics.Raycast(landingRay, out hit, 3))
            {
                if(footStepsAudioSrc != null) 
                {
                    switch(hit.collider.tag)
                    {
                        case "GrassFloor":
                            footStepsAudioSrc.PlayOneShot(grassSounds[Random.Range(0, grassSounds.Length -1 )]);
                            break;
                        case "ConcreteFloor":
                            footStepsAudioSrc.PlayOneShot(concreteSounds[Random.Range(0, concreteSounds.Length -1 )]);
                            break;
                        case "SandFloor":
                            footStepsAudioSrc.PlayOneShot(sandSounds[Random.Range(0, sandSounds.Length -1 )]);
                            break;
                        case "SnowFloor":
                            footStepsAudioSrc.PlayOneShot(snowSounds[Random.Range(0, snowSounds.Length -1 )]);
                            break;
                        default:
                            footStepsAudioSrc.PlayOneShot(concreteSounds[Random.Range(0, concreteSounds.Length -1 )]);
                            break;
                    }

                }
            }
            footStepTimer = 0.5f;
        }
    }

    // Update is called once per frame
    void Update()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        minimapCamera.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        minimapCamera.transform.position = new Vector3(transform.position.x, 450.0f, transform.position.z);

        if(useFootsteps) {
            HandleFootSteps();
        }
    }
}