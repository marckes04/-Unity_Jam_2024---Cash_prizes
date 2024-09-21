using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    public float playerSpeed = 1.9f;
    public float currentPlayerSpeed = 0f;
    public float playerSprint = 3f;

    [Header("Player Camera")]
    public Transform playerCamera;

    [Header("Player Animator and Gravity")]
    public CharacterController characterController;
    public float gravity = -9.81f;
    public Animator animator;

    [Header("Player Jumping & Velocity")]
    public float jumpRange = 1f;
    public float turnCalmTime = 0.1f;
    float turnCalmVelocity;
    Vector3 velocity;
    public Transform surfaceCheck;
    bool onSurface;
    public float surfaceDistance = 0.4f;
    public LayerMask surfaceMask;

    private void Start()
    {
        // Ensure the Animator component is properly linked
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        // Check if the player is grounded
        onSurface = Physics.CheckSphere(surfaceCheck.position, surfaceDistance, surfaceMask);

        if (onSurface && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Handle movement and jumping
        PlayerMove();
        Jump();
    }

    void PlayerMove()
    {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float verticalAxis = Input.GetAxisRaw("Vertical");

        // Create a movement direction vector based on input
        Vector3 direction = new Vector3(horizontalAxis, 0f, verticalAxis).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Set movement animations
            animator.SetBool("Walk", true);
            animator.SetBool("Running", Input.GetButton("Sprint"));
            animator.SetBool("Idle", false);

            // Determine if the player is sprinting or walking
            currentPlayerSpeed = (Input.GetButton("Sprint") && (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.UpArrow))) ? playerSprint : playerSpeed;

            // Get the camera's forward direction, ignoring the y-axis
            Vector3 cameraForward = new Vector3(playerCamera.forward.x, 0f, playerCamera.forward.z).normalized;
            Vector3 cameraRight = new Vector3(playerCamera.right.x, 0f, playerCamera.right.z).normalized;

            // Calculate movement direction relative to the camera
            Vector3 moveDirection = cameraForward * verticalAxis + cameraRight * horizontalAxis;

            // Only rotate the player to face the camera's forward direction when moving forward or backward
            if (Mathf.Abs(verticalAxis) > 0.1f)
            {
                float targetAngle = Mathf.Atan2(cameraForward.x, cameraForward.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            }

            // Move the player
            characterController.Move(moveDirection.normalized * currentPlayerSpeed * Time.deltaTime);
        }
        else
        {
            // Set idle animations when not moving
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Running", false);
        }
    }



    void Jump()
    {
        // Trigger jump animation and movement only when grounded
        if (Input.GetButtonDown("Jump") && onSurface)
        {
            animator.SetTrigger("Jump");
            velocity.y = Mathf.Sqrt(jumpRange * -2 * gravity);
        }
    }


    // Player Damage
    //Player Die

}
