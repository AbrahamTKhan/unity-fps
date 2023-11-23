using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MovementInput : MonoBehaviour
{
    private float speed; //Variables that determine the speed of the player whilst moving
    private float maxSpeed;
    public float maxWalkSpeed;
    public float maxRunSpeed;
    public float maxCrouchSpeed;
    public float maxBoostSpeed;
    public float maxSlideSpeed;
    public float boostCost;
    public float friction;

    public bool isGrappling;
    public bool isShooting;
    private bool canJump;
    private bool autoStand;
    public float jumpForce;
    public float airSpeed;
    public float jumpInterlude;
    public bool isGrounded;
    public bool isRunning;
    private bool isJumping;
    private bool isSliding;
    private bool carryMomentum;
    public bool isPaused;
    private bool walkAudioCheck;
    private bool runAudioCheck;
    bool idle;

    private SaveManager saveManager;
    private FaderScript fader;

    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip slideSound;
    private AudioSource audioSource;

    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject controlsMenu;

    public float inclineAngleLimit;
    private RaycastHit inclineHit;
    public bool isBoosting;
    float inputX;
    float inputY;
    Vector3 inputDirection;

    public StartGunAnimation pistolAnim;
    public StartGunAnimation rifleAnim;
    public StartGunAnimation shotgunAnim;
    public bool playingWalkAnim;

    private GameController gameController;

    [Header("Transforms")]
    public Transform playerTransform;

    Rigidbody playerRigidbody;

    [Header("State")]
    public PlayerMovement CurrentMovementState;
    PlayerMovement LastState;

    [Header("Layer")]
    public LayerMask Floor;

    public enum PlayerMovement //Enums to mimic player movement states
    {
        Walking,
        Running,
        Boosting,
        Sliding,
        Falling,
        Crouched
    }

    void Start() //Sets default values
    {
        fader = GameObject.FindGameObjectWithTag("Fader").GetComponent<FaderScript>();
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.freezeRotation = true;

        audioSource = GetComponent<AudioSource>();

        canJump = true;
        autoStand = false;
        playingWalkAnim = false;
        isPaused = false;
        isGrounded = true;
        walkAudioCheck = true;
        runAudioCheck = true;
        idle = true;
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) && !gameController.GameOver()) //Handles player pause input
        {
            if (isPaused)
            {
                ShowMenu(false);
            }
            else
            {
                ShowMenu(true);
            }
        }

        if (!isPaused && !gameController.GameOver()) //Allows player movement when appropriate
        {
            HandleJumpState();
            MovementStateMachine();
            Move();
        }

    }

    private void FixedUpdate()
    {
        inputDirection = playerTransform.forward * inputX + playerTransform.right * inputY; //Gets player's input direction
        inputDirection.Normalize();

        if (InclineCheck() && !isJumping) //Adjusts player's movement force if they're on a slope
        {
            Vector3 inclineAngle = Vector3.ProjectOnPlane(inputDirection, inclineHit.normal).normalized;
            playerRigidbody.AddForce(inclineAngle * speed, ForceMode.Force);
        }

        else if (isGrounded) //Applies normal force when on flat ground
        {
            playerRigidbody.AddForce(inputDirection * speed, ForceMode.Force);
        }

        else //Adjusts movement force when in the air
        {
            playerRigidbody.AddForce(inputDirection * speed * airSpeed, ForceMode.Force);
        }
    }

    public void ShowMenu(bool show) //Function for displaying/hiding menu objects
    {
        if (show)
        {
            isPaused = true;
            fader.SetCurrent(optionsMenu);
            fader.SetNext(pauseMenu);
            fader.StartCoroutine(fader.FadeScreen(1));
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else
        {
            isPaused = false;
            pauseMenu.GetComponent<MainMenu>().CallSave();
            fader.StartCoroutine(fader.FadeExit(pauseMenu, optionsMenu, controlsMenu));
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f; //Stops or resumes time scale depending on pause state
        }
    }

    private void MovementStateMachine() //Assigns player's current state and maximum speed depending on the input
    {
        isRunning = false;
        if (isBoosting) //When the player is boosting
        {
            CurrentMovementState = PlayerMovement.Boosting;
            speed = maxBoostSpeed;
            maxSpeed = 20f;
            float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? 7 : 11;
            maxSpeed = Mathf.Lerp(25, moveSpeed, 0.4f);
        }

        else if (!isGrounded) //When the player is in the air
        {
            CurrentMovementState = PlayerMovement.Falling;

            if (speed < maxRunSpeed) //Limits rigidbody speed
            {
                speed = maxWalkSpeed;
            }
            else
            {
                speed = maxRunSpeed;
            }
        }

        else if (isGrounded && isSliding) //When the player is sliding
        {
            CurrentMovementState = PlayerMovement.Sliding;
            speed = 300;
            maxSpeed = 15f;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && isGrounded) //When the player is crouched
        {
            CurrentMovementState = PlayerMovement.Crouched;
            speed = maxCrouchSpeed;
            maxSpeed = 4f;
        }
        else if (isGrounded && !isSliding && Input.GetKey(KeyCode.LeftShift)) //When the player is running
        {
            CurrentMovementState = PlayerMovement.Running;
            speed = maxRunSpeed;
            isRunning = true;
            maxSpeed = 15f;

            if ((runAudioCheck || LastState == PlayerMovement.Walking) && !idle)
            {
                runAudioCheck = false;
                audioSource.clip = runSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else //When the player is walking
        {
            CurrentMovementState = PlayerMovement.Walking;
            speed = maxWalkSpeed;
            maxSpeed = 7f;

            if ((walkAudioCheck || LastState == PlayerMovement.Running) && !idle)
            {
                walkAudioCheck = false;
                audioSource.clip = walkSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }

        if (autoStand && CurrentMovementState != PlayerMovement.Crouched) //Automatically stays crouched when under a low obstacle
        {
            speed = maxCrouchSpeed;
            maxSpeed = 4f;
        }

        if (isGrounded && autoStand && !Physics.Raycast(transform.position, transform.up, 1.5f)) //Automatically stands out of a low obstacle
        {
            autoStand = false;
            transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
        }
        if (idle && (!runAudioCheck || !walkAudioCheck) && !isShooting)
        {
            runAudioCheck = true;
            walkAudioCheck = true;
            audioSource.Stop();
            audioSource.loop = false;
        }

        LastState = CurrentMovementState;

    }

    private void Move()
    {
        inputX = Input.GetAxisRaw("Vertical"); //Gets the player's input direction
        inputY = Input.GetAxisRaw("Horizontal");

        CheckCrouched();

        bool notMoving = (inputX == 0) && (inputY == 0);
        idle = (notMoving || !isGrounded) && !isBoosting && !isGrappling && !isSliding;
        if (playingWalkAnim && !notMoving && isGrounded && !isShooting && !isSliding && !isGrappling) //Starts walking animation when moving on the ground
        {
            pistolAnim.WalkAnimation(true);
            rifleAnim.WalkAnimation(true);
            shotgunAnim.WalkAnimation(true);
            playingWalkAnim = false;
        }
        else if (!playingWalkAnim && (notMoving || !isGrounded) && !isBoosting && !isShooting && !isGrappling && !isSliding) //Stops moving animation if standing, in-air, boosting or shooting
        {

            pistolAnim.WalkAnimation(false);
            rifleAnim.WalkAnimation(false);
            shotgunAnim.WalkAnimation(false);
            playingWalkAnim = true;
        }


        if ((InclineCheck() && !isJumping) || isGrappling)
        {
            if (playerRigidbody.velocity.magnitude > maxSpeed)
            {
                playerRigidbody.velocity = playerRigidbody.velocity.normalized * maxSpeed; //Limits rigidbody speed
            }
        }

        else
        {
            Vector3 playerVelocity = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z); //Doesn't factor y velocity when calculating movement speed if on the ground

            if (playerVelocity.magnitude > maxSpeed)
            {
                Vector3 adjustedVelocity = playerVelocity.normalized * maxSpeed; //Adjusts x and z velocity if the rigidbody is moving over the maximum speed
                playerRigidbody.velocity = new Vector3(adjustedVelocity.x, playerRigidbody.velocity.y, adjustedVelocity.z);
            }
        }
    }

    private void CheckCrouched() //Handles crouching input
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded) //When the player crouches
        {
            autoStand = false;
            transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
            playerRigidbody.AddForce(Vector3.down * 8f, ForceMode.Impulse); //Adds a down force to get the rigidbody in crouch position

            if (Input.GetKey(KeyCode.LeftShift)) //If the player is running when crouching then initiates a slide
            {
                isSliding = true;
                walkAudioCheck = true;
                runAudioCheck = true;
                audioSource.clip = slideSound;
                audioSource.loop = true;
                audioSource.Play();

                if (InclineCheck()) //Adjusts slide boost based on surface angle
                {
                    Vector3 inclineAngle = Vector3.ProjectOnPlane(inputDirection, inclineHit.normal).normalized;
                    playerRigidbody.AddForce(inclineAngle * maxSlideSpeed, ForceMode.Impulse); //Adds a boost in speed when sliding
                }
                else
                {
                    playerRigidbody.AddForce(inputDirection * maxSlideSpeed, ForceMode.Impulse);
                }

                Invoke(nameof(SlideCooldown), 1f);

            }

        }

        if (Input.GetKeyUp(KeyCode.LeftControl)) //When the player stops crouching
        {
            if (Physics.Raycast(transform.position, transform.up, 1.5f))
            {
                autoStand = true;
            }
            else
            {
                transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
            }
        }
    }

    private void Jump()
    {
        isJumping = true;
        audioSource.PlayOneShot(jumpSound);
        playerRigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse); //Adds upwards force
    }

    private void HandleJumpState()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump && !isBoosting) //Activates jump
        {
            Jump();
            Invoke(nameof(JumpCooldownDelegate), jumpInterlude); //Cooldown gives player time to get off the ground

            canJump = false;
        }

        bool oldGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 2f * 0.5f + 0.2f, Floor); //Raycasts downwards to check if the player is on the ground or not

        if (!oldGrounded && isGrounded && playerRigidbody.velocity.y < -1)
        {
            audioSource.PlayOneShot(landSound);
        }

        if (isGrounded && canJump && !isBoosting && !isGrappling)
        {
            playerRigidbody.drag = friction; //Applies normal drag when grounded
        }
        else
        {
            playerRigidbody.drag = 0f; //Sets drag to 0 when jumping or using movement abilities
        }

        if (!isGrounded && isBoosting)
        {
            /* playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);*/
            playerRigidbody.useGravity = false; //Disables gravity whilst boosting
        }
        else
        {
            playerRigidbody.useGravity = true;
        }
    }

    private void JumpCooldownDelegate()
    {
        isJumping = false;
        canJump = true;
    }

    private void SlideCooldown()
    {
        isSliding = false;

        if (!isGrappling)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }

    private bool InclineCheck() //Checks the angle of the floor
    {
        if (Physics.Raycast(transform.position, Vector3.down, out inclineHit, 2f * 0.5f + 0.3f))
        {
            float floorAngle = Vector3.Angle(Vector3.up, inclineHit.normal);
            if (floorAngle != 0 && floorAngle < inclineAngleLimit) //Compares angle of the floor to the max angle set in the editor
            {
                return true; //Returns true if the floor is not flat and less than the limit
            }
            else
            {
                return false;
            }
        }

        return false; //Returns false if nothing was hit
    }

    public float GetVelocity()
    {
        return playerRigidbody.velocity.magnitude;
    }
}
