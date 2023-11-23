using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveAmmo : MonoBehaviour
{
    public float ammoBoost;
    private void OnTriggerEnter(Collider other) //Gives the player ammo if they move in to the ammo pickup
    {
        if (other.gameObject.tag == "Player"    )
        {
            other.GetComponentInParent<Shoot>().BoostAmmo(ammoBoost);
            gameObject.SetActive(false);
        }
    }
}
