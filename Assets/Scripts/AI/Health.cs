using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float health;
    private GameController gameController;
    public int enemyType;
    private bool isDead;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        health = maxHealth;
        isDead = false;
    }

    public void TakeDamage(float damageApplied) //Applies damage to enemy and checks for death
    {
        if (!isDead)
        {
            health -= damageApplied;
            if (health <= 0) // < 0 = dead
            {
                gameController.EnemyDead();
                isDead = true;
                if (enemyType == 1) //Handles death different based on what enemy type was killed
                {
                    int type = health < -30 ? 2 : 1;
                    GetComponent<PistolEnemyAI>().HandleDeath(type);
                }
                else if (enemyType == 2)
                {
                    int type = health < -30 ? 2 : 1;
                    GetComponent<EnemyAnimation>().HandleDeath(type);
                }
            }
        }
    }
}
