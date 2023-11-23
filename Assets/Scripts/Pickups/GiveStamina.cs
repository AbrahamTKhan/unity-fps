using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveStamina : MonoBehaviour
{
    public float staminaBoost;
    private void OnTriggerEnter(Collider other) //Gives the player stamina if they move in to the stamina pickup
    {
        if (other.gameObject.tag == "Player" && other.GetComponentInParent<UpdateUI>().stamina < other.GetComponentInParent<UpdateUI>().maxStamina)
        {
            other.GetComponentInParent<UpdateUI>().ChangeStamina(staminaBoost);
            gameObject.SetActive(false);
        }
    }
}
