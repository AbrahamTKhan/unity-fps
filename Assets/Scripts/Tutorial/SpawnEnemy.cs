using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject shooterEnemy;
    public GameObject runnerEnemy;
    public GameObject health;
    public GameObject stamina;
    public GameObject ammo;
    public GameObject spawnLocation;
    public int type;
    public void CreateEnemy() //Spawns an enemy, health pickup, stamina or ammo pickup
    {
        if (type == 1)
        {
            Instantiate(shooterEnemy, spawnLocation.transform);
        }
        if (type == 2)
        {
            Instantiate(runnerEnemy, spawnLocation.transform);
        }
        else if (type == 3)
        {
            Instantiate(health, spawnLocation.transform);
        }
        else if (type == 4)
        {
            Instantiate(stamina, spawnLocation.transform);
        }
        else if (type == 5)
        {
            Instantiate(ammo, spawnLocation.transform);
        }
    }
}
