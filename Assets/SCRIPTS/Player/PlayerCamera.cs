using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UIElements;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] Camera playerCamera = null;
    [SerializeField] GameObject pivot = null;
    [SerializeField] float mouseSensitivityY = 60f;
    [SerializeField] RefereeSystem refereeSystem;




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
        else { refereeSystem = GameObject.FindGameObjectWithTag("RefereeSystem").GetComponent<RefereeSystem>(); }
    }

    void Update()
    {
        if (!hasAuthority) { return; }

        // Disable camera if not in play
        if (!refereeSystem.isInPlay()) { return; }
        
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

        cameraVerticalRotation += inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -25f, 25f);
        pivot.transform.localEulerAngles = camAxis * cameraVerticalRotation;
    }
}
