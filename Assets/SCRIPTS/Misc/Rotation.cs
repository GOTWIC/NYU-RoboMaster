using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] public float rotationSpeed = 30f;

    void Update()
    {
        transform.RotateAround(transform.parent.position, new Vector3(0, 1, 0), rotationSpeed * Time.deltaTime);
    }
}
