using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public Camera playerCam;
    private Rigidbody playerRigidbody;
    private RaycastHit hitObject;
    private Vector3 hitLocation;
    private MovementInput playerMovement;
    private UpdateUI updateUI;
    float autoStopDistance;
    public float grappleCost;
    private bool isGrappling;
    public AudioClip reelSound;
    private AudioSource audioSource;
    private LineRenderer grappleLine;

    private void Start()
    {
        grappleLine = GetComponent<LineRenderer>();
        grappleLine.positionCount = 0;
        playerRigidbody = GetComponent<Rigidbody>();
        playerMovement = GetComponent<MovementInput>();
        updateUI = GetComponent<UpdateUI>();
        audioSource = GetComponent<AudioSource>();
        hitLocation = transform.position;
        autoStopDistance = 0;
        isGrappling = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && !isGrappling && updateUI.stamina > grappleCost) //Activates grappling
        {
            LaunchHook();
        }

        if (playerMovement.isGrappling && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))) //Stops grappling if player boosts or jumps
        {
            StopCoroutine(BlendHook());
        }
    }

    void LaunchHook() //Pulls player in the direction they're facing
    {
        bool objectHit = Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hitObject, 40); //Limits range to 40m
        if (objectHit)
        {
            audioSource.clip = reelSound;
            audioSource.loop = true;
            audioSource.Play();

            isGrappling = true;
            updateUI.ChangeStamina(-grappleCost);
            autoStopDistance = 0;
            grappleLine.positionCount = 2;
            /*playerMovement.transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);*/
            playerMovement.isGrappling = true;
            hitLocation = new Vector3(hitObject.point.x, hitObject.point.y, hitObject.point.z); //Gets location of the hit
            StartCoroutine(BlendHook());

            RaycastHit sweepHit;
            if (playerRigidbody.SweepTest(playerCam.transform.forward, out sweepHit, 40)) //Unused auto stop feature
            {
                autoStopDistance = Vector3.Distance(sweepHit.transform.position, playerRigidbody.transform.position);
            }

            /*
            Vector3 hitDirection = hitLocation - transform.position;
            hitDirection = hitDirection.normalized * 30;
            playerRigidbody.AddForce(hitDirection, ForceMode.Impulse);*/
        }
    }

    private void LateUpdate()
    {
        if (isGrappling)
        {
            Vector3 grapplePoint = Vector3.Lerp(transform.position, hitLocation, Time.deltaTime * 8f);
            grappleLine.SetPosition(0, transform.position);
            grappleLine.SetPosition(1, hitLocation);
        }
        
    }

    public IEnumerator BlendHook() //Smoothly moves player to hit location
    {
        float blend = 0.025f;
        double time = 0;
        Vector3 startGrappleLocation = playerRigidbody.transform.position;
        while (Vector3.Distance(playerRigidbody.transform.position, hitLocation) > 4) //Stops once player is close to hitlocation
        {

            /*if (autoStopDistance != 0 && Vector3.Distance(startGrappleLocation, playerRigidbody.transform.position) >= autoStopDistance - 4)
            {
                break;
            }*/

            time += 1 * Time.deltaTime;
            if (time >= 0.01)
            {
                playerMovement.transform.position = Vector3.Lerp(playerMovement.transform.position, hitLocation, blend);
                blend += 0.01f * Time.deltaTime;
                time = 0f;
            }
            
            yield return null;
        }

        audioSource.loop = false;
        audioSource.Stop();
        grappleLine.positionCount = 0;
        isGrappling = false;
        playerMovement.isGrappling = false;
    }
}
