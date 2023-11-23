using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    void Update() //Rotates the pickups
    {
        transform.Rotate(new Vector3(0, 120, 0) * Time.deltaTime);
    }
}
