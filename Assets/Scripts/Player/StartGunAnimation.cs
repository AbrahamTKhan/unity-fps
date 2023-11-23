using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGunAnimation : MonoBehaviour
{
    public MovementInput playerMovement;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerMovement.isRunning && !playerMovement.isShooting) //Adjusts movement animation speed depending on state
        {
            anim.speed = 1.5f;
        }
        else if (playerMovement.CurrentMovementState == MovementInput.PlayerMovement.Crouched && !playerMovement.isShooting) 
        {
            anim.speed = 0.5f;
        }
        else
        {
            anim.speed = 1f;
        }
    }

    public void ShootAnimation() //Plays shooting animation
    {
        anim.enabled = true;
        anim.speed = 1;
        anim.Play("ShootGun");
    }

    public void WalkAnimation(bool shouldPlay) //Plays walking animation
    {
        if (shouldPlay)
        {
            anim.enabled = true;
            anim.Play("Walk");
        }
        else
        {
            anim.enabled = false;
        }
        
        
    }

    public void BoostAnimation() //Plays boost animation
    {
        anim.enabled = true;
        anim.Play("Boost");
    }
}
