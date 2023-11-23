using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    private Quaternion newRotation;
    public IEnumerator ShakeCamera(float shakeLength, float shakeStrength) //Adds small shake when player shoots
    {
        
        float timer = 0;
        while (timer < shakeLength)
        {
            timer += Time.deltaTime;
            Quaternion cameraPosition = transform.localRotation;
            newRotation = new Quaternion(cameraPosition.x - 0.01f, cameraPosition.y, cameraPosition.z, cameraPosition.w);
            transform.localRotation = Quaternion.Slerp(cameraPosition, newRotation, 0.5f);
            yield return null;
        }
    }

}
