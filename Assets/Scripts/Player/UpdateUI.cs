using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UpdateUI : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public float stamina;
    public float maxStamina;
    public float staminaRegenRate;

    private GameController gameController;
    private MovementInput movementInput;

    public Scrollbar healthBar;
    public Scrollbar staminaBar;

    public Image bloodSplatter;
    void Start()
    {
        int difficulty = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GetDifficulty(); //Gets difficulty from game controller

        if (difficulty == 0) //Assigns values depending on difficulty
        {
            maxHealth = 500;
            stamina = 200;
            staminaRegenRate = 7.5f;
        }
        else if (difficulty == 1)
        {
            maxHealth = 3000;
            stamina = 1000;
            staminaRegenRate = 100;
        }
        else if (difficulty == 3)
        {
            maxHealth = 300;
            stamina = 75;
            staminaRegenRate = 2.5f;
        }
        else
        {
            maxHealth = 450;
            stamina = 100;
            staminaRegenRate = 5;
        }

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        movementInput = GetComponent<MovementInput>();

        health = maxHealth;
        healthBar.size = health;

        stamina = maxStamina;
        staminaBar.size = stamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (stamina < maxStamina) //Slowly adds stamina every frame
        {
            stamina += staminaRegenRate * Time.deltaTime;
        }

        staminaBar.size = stamina / maxStamina;
    }

    public void TakeDamage(float damageApplied, int type, float distance) //Lowers health and checks for death if hit
    {
        if (!movementInput.isBoosting)
        { 
            float hitChance = Random.Range(0.2f, 1.01f); //Randomises chance of getting hit
            if (type == 1)
            {
                float playerSpeed = movementInput.GetVelocity();
                if (playerSpeed >= 15) { playerSpeed = 0; }
                else if (playerSpeed <= 0) { playerSpeed = 1; }
                else 
                { 
                    playerSpeed = 1 - (playerSpeed / 20);
                    playerSpeed = playerSpeed < 0 ? 0 : playerSpeed; //Lowers chance of getting hit the faster the player is moving
                }

                if (distance >= 50) { distance = 0.1f; }
                else if (distance <= 5) { distance = 2; } //Lowers chance of getting hit the further the player is from the shooter
                else 
                { 
                    distance = 1 - (distance / 150);
                    distance = distance < 0 ? 0 : distance * 2;
                }

                hitChance *= playerSpeed * distance;
            }
            
            if (hitChance > 0.5f || type == 2) //Only applies damage if chance is satisfied
            {
                health -= damageApplied;
                healthBar.size = health / maxHealth;
                if (health <= 0) //Checks if player died
                {
                    gameController.PlayerDead();
                }
                StopAllCoroutines();
                Color bloodColour = new Color(256, 256, 256, 64f);
                bloodSplatter.color = bloodColour; //Displays bloof effect
                StartCoroutine(BloodUIFade());
            }
        }
    }

    public void ChangeHealth(float value) //Adjusts health
    {
        health += value;
        healthBar.size = health / maxHealth;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void ChangeStamina (float value) //Adjusts stamina
    {
        stamina += value;
        staminaBar.size = stamina / maxStamina;
        if (stamina > maxStamina)
        {
            stamina = maxStamina;
        }
    }

    IEnumerator BloodUIFade() //Fades blood in to screen
    {
        yield return new WaitForSeconds(0.5f);

        Color bloodColour = new Color(256, 256, 256, 64);
        while (true)
        {
            bloodColour.a = Mathf.Lerp(bloodColour.a, 0, 0.8f);
            bloodSplatter.color = bloodColour;

            if (bloodColour.a < 0.01)
            {
                bloodColour.a = 0;
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
