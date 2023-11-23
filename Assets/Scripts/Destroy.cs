using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    private Light impactLight;
    private float maxSize;
    private float sizeIncrease;
    private float intensityIncrease;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioSource.clip);
        impactLight = GetComponent<Light>();
        int type = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<Shoot>().gunType;
        if (type == 1) //Sets stats of impact light depending on what gun was used
        {
            sizeIncrease = 0.05f;
            intensityIncrease = 2f;
            maxSize = 0.35f;
            impactLight.color = new Color(0.07843138f, 0.5960785f, 0);
        }
        else if (type == 2)
        {
            sizeIncrease = 0.05f;
            intensityIncrease = 2f;
            maxSize = 0.5f;
        }
        else
        {
            sizeIncrease = 0.15f;
            intensityIncrease = 3f;
            impactLight.range = 0.85f;
            maxSize = 1.75f;
            impactLight.color = new Color(0, 0.03921569f, 0.5960785f);
        }
        
        StartCoroutine(ChangeSize());

        Invoke(nameof(DeleteLight), 1);
    }

    private void DeleteLight()
    {
        gameObject.SetActive(false);
    }

    IEnumerator ChangeSize() //Smoothly increases hit impact size until it needs to be deleted
    {
        double timer = 0;
        while (true)
        {
            timer += 1 * Time.deltaTime;

            if (timer > 0.01)
            {
                impactLight.range += sizeIncrease;
                impactLight.intensity += intensityIncrease;
                timer = 0;
            }

            if (impactLight.range > maxSize)
            {
                impactLight.enabled = false;
                break;
            }

            yield return null;
        }
    }

}
