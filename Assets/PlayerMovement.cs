using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f; 
    [SerializeField] private bool useFootsteps = true;
    [SerializeField] private AudioSource footStepsAudioSrc = default;
    [SerializeField] private AudioClip[] grassSounds = default;
    [SerializeField] private AudioClip[] pathSounds = default;
    private float footStepTimer = 0;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    public Transform minimapCamera;
    public Camera playerCamera;
    private Vector2 currentInput;



    private void HandleFootSteps()
    {
        currentInput = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal") ); 

        if(!controller.isGrounded) return;
        if(currentInput == Vector2.zero) return;
        footStepTimer -= Time.deltaTime;

        if(footStepTimer <= 0) {
            if(Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, 3))
            {
                switch(hit.collider.tag)
                {
                    case "GrassFloor":
                        footStepsAudioSrc.PlayOneShot(grassSounds[0]);
                        break;
                    default:
                        footStepsAudioSrc.PlayOneShot(grassSounds[0]);
                        break;

                }
            }
            footStepTimer = 0.7f;
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