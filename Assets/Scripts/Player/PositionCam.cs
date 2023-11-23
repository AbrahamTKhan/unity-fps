using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCam : MonoBehaviour
{
    public Transform camPosition;
    void Update() //Updates camera position to the player's
    {
        transform.position = camPosition.position;
    }
}
