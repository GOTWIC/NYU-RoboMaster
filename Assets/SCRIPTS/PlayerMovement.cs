using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] float movementSpeed = 60f;
    [SerializeField] float mouseSensitivityX = 60f;

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
        CMDRotate(inputX);

    }

    private void tryToTranslate()
    {
        if (Input.GetKey(KeyCode.W))
        { CMDTranslate(Vector3.forward); }

        if (Input.GetKey(KeyCode.S))
        { CMDTranslate(Vector3.back); }

        if (Input.GetKey(KeyCode.A))
        { CMDTranslate(Vector3.left); }

        if (Input.GetKey(KeyCode.D))
        { CMDTranslate(Vector3.right); }
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
