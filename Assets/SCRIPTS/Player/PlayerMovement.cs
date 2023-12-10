using UnityEngine;
using Mirror;
using System.Net;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] float acceleration = 1f;
    [SerializeField] float topSpeed = 10f;
    [SerializeField] float mouseSensitivityX = 60f;
    [SerializeField] Health health = null;
    [SerializeField] LayerMask layerMask;

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
        ClientRotate(inputX);
    }

    private void tryToTranslate()
    {
        if (Input.GetKey(KeyCode.W))
        { ClientTranslate(transform.forward); }

        if (Input.GetKey(KeyCode.S))
        { ClientTranslate(-transform.forward); }

        if (Input.GetKey(KeyCode.A))
        { ClientTranslate(-transform.right); }

        if (Input.GetKey(KeyCode.D))
        { ClientTranslate(transform.right); }
    }

    private void ClientTranslate(Vector3 vec)
    {
        // If the robot is dead, it cannot translate (but can rotate)
        if (health.inDeathState()) { return; }

        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > topSpeed) { return; }

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(-transform.up), out hit, 1, layerMask))
        {
            gameObject.GetComponent<Rigidbody>().AddForce(vec * acceleration * Time.deltaTime * 10000000);
        }
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1, Color.yellow);

    }

    private void ClientRotate(float inp)
    {
        this.transform.Rotate(Vector3.up * inp);
    }
}
