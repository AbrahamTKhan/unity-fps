using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PistolEnemyAI : MonoBehaviour
{
    private Transform movePositionTransform;
    private NavMeshAgent navMeshAgent;
    public float initialWait;
    public float shootCooldown;
    public float shotError;
    private RaycastHit hitObject;
    private Vector3 aimVariation;

    private AudioSource audioSource;
    public AudioClip pistolSound;

    public Light gunFlash;

    private Animator anim;
    private bool isDead;

    public enum AIStateMachine //Currently unused AI state enum
    {
        Idle,
        Patrol,
        Follow,
        Attack
    }
    private void Awake() //Assigns components 
    {
        isDead = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        movePositionTransform = GameObject.Find("PlayerCharacter").transform;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
    }
    void Start()
    {
        StartCoroutine(Fire()); //Starts repeating shoot coroutine
        aimVariation = new Vector3(0, 0, 0);
        randomiseAim();
        gunFlash.enabled = false;
        
    }

    void Update()
    {
        if (!isDead)
        {
            if (TargetDistance() > 10) //Moves the AI to the player until its within 5m
            {
                navMeshAgent.destination = movePositionTransform.position;
                if (navMeshAgent.velocity.magnitude < 0.1f)
                {
                    anim.speed = 0.01f;
                }
                else
                {
                    anim.speed = 1f;
                }
            }
            else
            {
                navMeshAgent.destination = transform.position;
                anim.speed = 0.01f;
            }

            Vector3 moveY = new Vector3(movePositionTransform.position.x, transform.position.y, movePositionTransform.position.z);
            transform.LookAt(moveY); //Rotates the AI to face the player
        }
    }

    private float TargetDistance()
    {
        return Vector3.Distance(movePositionTransform.position, transform.position);
    }

    private IEnumerator Fire() //Coroutine for shooting
    {
        float timer = 0;
        float randShoot = Random.Range(-1, 1); //Randomises shot timing
        while (!isDead)
        {
            timer += 1 * Time.deltaTime;
            
            if (timer + randShoot >= shootCooldown) //Attempts shot once time elapsed + randomised time is greater than predifined cooldown
            {
                randShoot = Random.Range(-1, 1);

                audioSource.PlayOneShot(pistolSound, 0.3f); //Plays sound and enabled muzzle flash light
                gunFlash.enabled = true;

                StartCoroutine(ChangeSize());
                Vector3 aiPos = transform.position;
                aiPos.y += 1.5f;
                bool objectHit = Physics.Raycast(aiPos, movePositionTransform.position - aiPos, out hitObject, Mathf.Infinity);
                Vector3 playerPos = movePositionTransform.position;
                playerPos.y -= 0.5f;
                
                Debug.DrawRay(aiPos, (movePositionTransform.position - aiPos) * 1000, Color.red, 1);
                if (objectHit && hitObject.collider.tag == "Player") //Checks raycast for player hit
                {
                    hitObject.collider.GetComponentInParent<UpdateUI>().TakeDamage(20f, 1, TargetDistance()); //Damages player if hit
                }
                timer = 0;
            }
            

            yield return null; //Returns until next frame
        }
    }

    private Vector3 randomiseAim() //Adds variance to AI aim, may miss shot
    {
        Vector3 direction = movePositionTransform.position - transform.position;
        float randX = Random.Range(-shotError, shotError);
        float randY = Random.Range(-shotError, shotError);
        float randZ = Random.Range(-shotError, shotError);
        aimVariation = Quaternion.Euler(randX, randY, randZ) * direction.normalized;
        return aimVariation;
    }

    public void HandleDeath(int type)
    {
        isDead = true;
        
        gunFlash.enabled = false;
        navMeshAgent.enabled = false;
        GetComponent<Collider>().enabled = false;
        StopCoroutine(Fire());
        anim.speed = 1;
        if (type == 1)
        {
            anim.Play("Death Animation 1");
        }
        else
        {
            anim.Play("Death Animation 2");
        }
        Invoke(nameof(DeathDelegate), 8);
    }

    private void DeathDelegate()
    {
        gameObject.SetActive(false);
    }

    IEnumerator ChangeSize() //Smoothly increases size of impact light until it disappears
    {
        double timer = 0;
        while (true)
        {
            timer += 1 * Time.deltaTime;

            if (timer > 0.01)
            {
                gunFlash.range -= 0.1f;
                timer = 0;
            }

            if (gunFlash.range < 0.5f)
            {
                gunFlash.range = 0.75f;
                gunFlash.enabled = false;
                break;
            }

            yield return null;
        }
    }
}
