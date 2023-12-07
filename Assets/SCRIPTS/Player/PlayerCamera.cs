using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] Camera playerCamera = null;
    [SerializeField] GameObject pivot = null;
    [SerializeField] float mouseSensitivityY = 60f;

    float cameraVerticalRotation = 0f;

    [SerializeField] Vector3 camAxis = new Vector3(0, 1, 0);

    // Start is called before the first frame update
    void Start()
    {
        if (!hasAuthority)
        {
            playerCamera.enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
        }
    }

    void Update()
    {
        if (!hasAuthority) { return; }

        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

        cameraVerticalRotation += inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -25f, 25f);
        pivot.transform.localEulerAngles = camAxis * cameraVerticalRotation;
    }
}
