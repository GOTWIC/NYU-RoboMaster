using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{

    [SerializeField] float movementSpeed = 60f;
    [SerializeField] float mouseSensitivityX = 60f;

    float cameraVerticalRotation = 0f;



    void Update()
    {
        if (hasAuthority)
        {
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
        CMDRotate(inputX);

    }

    private void tryToTranslate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            //this.transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            CMDTranslate(Vector3.forward);
        }

        if (Input.GetKey(KeyCode.S))
        {
            //this.transform.Translate(Vector3.back * movementSpeed * Time.deltaTime);
            CMDTranslate(Vector3.back);
        }

        if (Input.GetKey(KeyCode.A))
        {
            //this.transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
            CMDTranslate(Vector3.left);
        }

        if (Input.GetKey(KeyCode.D))
        {
            //this.transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
            CMDTranslate(Vector3.right);
        }
    }

    [Command]
    private void CMDTranslate(Vector3 vec)
    {
        this.transform.Translate(vec * movementSpeed * Time.deltaTime);
    }

    [Command]
    private void CMDRotate(float inp)
    {
        this.transform.Rotate(Vector3.up * inp);
    } 
}
