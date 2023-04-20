using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Room") { return; }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
