using UnityEngine;
using System.Collections;

public class EnemyAnimation : MonoBehaviour
{
	private UnityEngine.AI.NavMeshAgent nav;
	private Animator anim;

	private GameObject target;

	public AudioClip meleeSound;
	private AudioSource audioSource;

	private bool isAttacking;
	public float speedDampTime = 0.1f;
	public float deadZone = 5f;
	
	public float angularSpeedDampTime = 0.7f;
	public float angleResponseTime = 0.6f; 

	private float angle;
	private bool isDead;

	void Awake()
	{
		nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
		anim = GetComponent<Animator>();

		audioSource = GetComponent<AudioSource>();

		isDead = false;
		target = GameObject.Find("PlayerCharacter");

		nav.updateRotation = false; //Allows rootmotion to take over
		isAttacking = false;

		anim.SetLayerWeight(1, 1f);
		anim.SetLayerWeight(2, 1f); //Sets the weight of the layers

		deadZone *= Mathf.Deg2Rad; //Changes deadzone to radians
	}


	void Update()
	{
		if (target != null && !isDead)
		{
			angle = GetTargetAngle(transform.forward, nav.desiredVelocity); //Gets the angle to the target
			if (!isAttacking && (TargetDistance() > 3 || Mathf.Abs(angle) > 0.2)) //If they're far and not facing the right direction
            {
				nav.speed = 10;
				nav.SetDestination(target.transform.position);

				float speed = 0;
				float angularSpeed = 0;
				EvaluateAngle(out speed, out angularSpeed);
				
				anim.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
				anim.SetFloat("AngularSpeed", angularSpeed, angularSpeedDampTime, Time.deltaTime); // Activates run
			}
			if (!isAttacking && TargetDistance() <= 6 && Mathf.Abs(angle) > 0.2) //If they're close but not facing
			{
				nav.speed = 1; //Slow turn
			}
			else if (!isAttacking && TargetDistance() <= 3 && Mathf.Abs(angle) <= 0.2) //If they're close and facing the target
            {
				nav.speed = 0;
				isAttacking = true;
				transform.LookAt(target.transform);
				anim.applyRootMotion = false;
				anim.Play("Attack");
				Invoke(nameof(AttackDelegate), 1); //Attack target
				Invoke(nameof(LocomotionDelegate), 2);
			}
			
		}
	}

	private float TargetDistance()
    {
		return Vector3.Distance(target.transform.position, transform.position);

	}

	private void AttackDelegate() //Acts as frame for dealing damage
    {
		audioSource.PlayOneShot(meleeSound);

		if (TargetDistance() < 4) //Checks if target is still close
        {
			target.GetComponent<UpdateUI>().TakeDamage(40, 2, TargetDistance());
        }
	}

	public void HandleDeath(int type) //Appropriately deals with death
    {
		isDead = true;
		nav.speed = 0;
		nav.angularSpeed = 0;
		GetComponent<Collider>().enabled = false;
		CancelInvoke();

		if (type == 1) //Different animations for different gun deaths
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

	private void LocomotionDelegate() //Goes back to running after atttacking
    {
		isAttacking = false;
		anim.Play("Locomotion");
	}

	void OnAnimatorMove() //Changes movement based on position, rootmotion and delta time
	{
		if (Time.timeScale == 1 && Time.deltaTime != 0)
        {
			nav.velocity = anim.deltaPosition / Time.deltaTime;

			transform.rotation = anim.rootRotation;
		}
		
	}


	void EvaluateAngle(out float speed, out float angularSpeed)
	{
		speed = nav.desiredVelocity.magnitude;

		float angle = GetTargetAngle(transform.forward, nav.desiredVelocity);
        if (Mathf.Abs(angle) < deadZone)
        {
            transform.LookAt(transform.position + nav.desiredVelocity); //Locks on to target if they're within deadzone
            angle = 0f;
        }

        angularSpeed = angle / angleResponseTime; //Otherwise calculate the angle that it turn based on desired velocity
	}


	float GetTargetAngle(Vector3 fromVector, Vector3 toVector) //Gets the angle to the target from the AI
	{

		if (toVector == Vector3.zero) { return 0f; }

		float angle = Vector3.Angle(fromVector, toVector);
		angle = (Mathf.PI / 180) * angle; //Converts angle to radians

		Vector3 targetDirection = (fromVector - toVector).normalized;
		Vector3 cross = Vector3.Cross(targetDirection, fromVector);

		if (cross.y < 0)
        {
			angle *= -1; //Inverts angle if the target is in the other direction
        }

		return angle;
	}
}
