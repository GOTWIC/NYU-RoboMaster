using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
        
    [SerializeField] float movementSpeed = 60f;
    [SerializeField] float mouseSensitivityX = 60f;

    float cameraVerticalRotation = 0f;

    

    void Update()
    {
        if (hasAuthority) { 
            move();
        }   
    }

    private void move()
    {
        tryToRotate();
        tryToTranslate();
    }

    private void tryToRotate()
    {
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime * 10;
        //float inputY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

        //cameraVerticalRotation -= inputY;
        //cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        //this.transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        this.transform.Rotate(Vector3.up * inputX);
    }

    private void tryToTranslate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(Vector3.back * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
        }
    }
}
