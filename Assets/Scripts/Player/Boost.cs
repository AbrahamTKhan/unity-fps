using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Boost : MonoBehaviour
{
    public Transform playerTransform;
    public Transform cameraTransform;
    public Camera playerCamera;
    public StartGunAnimation pistolAnim;
    public StartGunAnimation rifleAnim;
    public StartGunAnimation shotgunAnim;
    private Rigidbody playerRigidbody;
    private MovementInput playerMovement;
    private UpdateUI updateUI;

    public float boostSpeed;
    public float boostTime;
    private Vector3 boostDirection;
    private AudioSource boostSource;
    public AudioClip boostSound;

    public float boostCooldown;
    public float boostCost;
    private bool canBoost;
    private bool resetFOV;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerMovement = GetComponent<MovementInput>();
        updateUI = GetComponent<UpdateUI>();
        boostSource = GetComponent<AudioSource>();

        canBoost = true;
        resetFOV = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canBoost && updateUI.stamina > boostCost) //Activates boost if input, cooldown and stamina cost are satisfied
        {
            canBoost = false;
            updateUI.ChangeStamina(-boostCost); //Lowers player's stamina
            StartCoroutine(SmoothFOV(0.1f));
            ActivateBoost();
            int currentGun = GetComponent<Shoot>().gunType;
            if (currentGun == 1) { pistolAnim.BoostAnimation(); }
            if (currentGun == 2) { rifleAnim.BoostAnimation(); }
            if (currentGun == 3) { shotgunAnim.BoostAnimation(); }
        }
    }

    public IEnumerator SmoothFOV(float speed) //Changes player's FOV whilst boosting
    {
        double timer = 0;
        while (true)
        {
            timer += 1 * Time.deltaTime;

            if (playerCamera.fieldOfView > 75) //Return FOV to normal once it's past 75
            {
                resetFOV = true;
            }

            if (timer >= 0.01f)
            {
                if (!resetFOV)
                {
                    playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 90, 0.0625f);
                }
                else
                {
                    playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 60, 0.125f);
                }
                timer = 0;
            }

            
            if (canBoost) //Stop once the boost delegate has run
            {
                StopAllCoroutines();
            }

            yield return null;
        }
        
    }

    private void ActivateBoost()
    {
        boostSource.PlayOneShot(boostSound);
        float inputX = Input.GetAxisRaw("Vertical");
        float inputY = Input.GetAxisRaw("Horizontal");

        Vector3 boostInputDirection = boostInputDirection = playerTransform.forward * inputX + playerTransform.right * inputY; //Boosts in the player's input direction
        
        if (inputX == 0 && inputY == 0)
        {
            boostInputDirection = playerTransform.forward; //Boosts forward if the player isn't moving
        }

        playerMovement.isBoosting = true;
        boostDirection = boostInputDirection.normalized * boostSpeed;
        Invoke(nameof(ApplyDelayedBoost), 0.025f);

        Invoke(nameof(BoostDelegate), boostTime);
    }

    private void ApplyDelayedBoost() //Delays boost because of rigidbody bug
    {
        playerRigidbody.AddForce(boostDirection, ForceMode.Impulse);
    }

    private void BoostDelegate() //Resets values once boost is done
    {
        playerMovement.isBoosting = false;
        playerMovement.playingWalkAnim = true;
        canBoost = true;
        resetFOV = false;
        playerCamera.fieldOfView = 60;
    }
}
