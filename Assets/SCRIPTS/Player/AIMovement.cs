using UnityEngine;
using Mirror;
using System.Net;
using static UnityEngine.GraphicsBuffer;
using System.IO;
using UnityEngine.AI;

public class AIMovement : NetworkBehaviour
{
    [SerializeField] float acceleration = 1f;
    [SerializeField] float topSpeed = 10f;
    [SerializeField] float mouseSensitivityX = 60f;
    [SerializeField] Health health = null;
    [SerializeField] LayerMask layerMask;

    private float topSpeedMulti = 1f;
    private float accelMulti = 1f;

    public Transform target;
    private NavMeshPath path;
    public float elapsed = 0.0f;

    private void Start()
    {
        path = new NavMeshPath();
    }

    void Update()
    {

        elapsed += Time.deltaTime;
        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
        }
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);


        move();
    }

    private void move()
    {
        tryToRotate();

    }

    public void OnCollisionStay(Collision collision)
    {
        tryToTranslate();
    }

    private void tryToRotate()
    {
        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

        Vector3 directionToNextCorner = (transform.position - path.corners[1]).normalized;
        Vector3 currentlyFacedDirection = this.transform.rotation.eulerAngles;

        transform.LookAt(directionToNextCorner);

        Debug.Log("direction to next corner" + directionToNextCorner);
        Debug.Log("currently faced direction" + currentlyFacedDirection);
        //this.transform.Rotate(directionToNextCorner - currentlyFacedDirection);


        //float turnDir = path.corners[0];

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

        if (Input.GetKey(KeyCode.Q))
        { health.dealDamage(1000); }
    }

    private void ClientTranslate(Vector3 vec)
    {
        // If the robot is dead, it cannot translate (but can rotate)
        if (health.inDeathState()) { return; }

        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > topSpeed * topSpeedMulti) { return; }

        gameObject.GetComponent<Rigidbody>().AddForce(vec * acceleration * accelMulti * Time.deltaTime * 10000000);

        /*
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(-transform.up), out hit, 1, layerMask))
        {
        }
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1, Color.yellow);
        */
    }

    private void ClientRotate(float inp)
    {
        this.transform.Rotate(Vector3.up * inp);
    }
    public void setTopSpeedMulti(float multi)
    {
        topSpeedMulti = multi;
    }

    public void setAccelMulti(float multi)
    {
        accelMulti = multi;
    }

}
