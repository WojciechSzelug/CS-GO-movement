using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    private PlayerControls playerControls;

    public Transform playerBody;
    public float sensitive = 100f;
    

    float xRotation = 0f;
    float yRotation = 0f;
    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void FixedUpdate()
    {
        Vector2 look = playerControls.Land.CameraRotation.ReadValue<Vector2>();

      
        xRotation -= look.y* sensitive * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
 
        yRotation = look.x * sensitive * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * yRotation);
    }
    

   

}
