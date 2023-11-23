using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraInput : MonoBehaviour
{
    public Shake camShake;
    private float camSensitivityX;
    private float camSensitivityY;
    float camRotationX;
    float camRotationY;
    public Transform cameraTransform;
    public MovementInput playerMovement;
    public Slider sensSlider;

    private void Awake()
    {
        camSensitivityX = camSensitivityY = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>().GetSensitivity();
    }
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; //Stops cursor from showing or moving once focused on the window
    }

    public void SetSensivity(float value)
    {
        camSensitivityX = camSensitivityY = sensSlider.value;
    }

    void Update() //Handles player mouse movements
    {
        if (!playerMovement.isPaused)
        {
            float playerMInputX = Input.GetAxisRaw("Mouse X") * camSensitivityX;
            float playerMInputY = Input.GetAxisRaw("Mouse Y") * camSensitivityY; //Adjusts input with sensitivity

            camRotationX -= playerMInputY; //Rotates the camera based on input
            camRotationY += playerMInputX;

            if (camRotationX < -90f) //Limits camera rotation
            {
                camRotationX = -90f;
            }
            else if (camRotationX > 90f)
            {
                camRotationX = 90f;
            }

            transform.rotation = Quaternion.Euler(camRotationX, camRotationY, 0);
            cameraTransform.rotation = Quaternion.Euler(0, camRotationY, 0);
        }
        
    }

}
