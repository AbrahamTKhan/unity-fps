using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveHealh : MonoBehaviour
{
    public float healthBoost;
    private void OnTriggerEnter(Collider other) //Gives the player health if they move in to the health pickup
    {
        if (other.gameObject.tag == "Player" && other.GetComponentInParent<UpdateUI>().health < other.GetComponentInParent<UpdateUI>().maxHealth)
        {
            other.GetComponentInParent<UpdateUI>().ChangeHealth(healthBoost);
            gameObject.SetActive(false);
        }
    }
}
