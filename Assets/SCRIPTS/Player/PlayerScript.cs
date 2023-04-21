using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerScript : NetworkBehaviour
{

    [SyncVar]
    [SerializeField] GameObject robot = null;

    [SerializeField] RefereeSystem refereeSystem = null;

    private void Start()
    {
        refereeSystem = GameObject.FindGameObjectWithTag("RefereeSystem").GetComponent<RefereeSystem>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Room") { return; }

        if (Input.GetKeyDown(KeyCode.Escape) && !refereeSystem.isGameEndActive() && hasAuthority) { refereeSystem.togglePause(); }

        if (Input.GetKeyDown(KeyCode.Delete) && hasAuthority) { Application.Quit(); }

        /*
        if (refereeSystem.isPaused()) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        */

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void setRobot(GameObject robot)
    {
        this.robot = robot;
    }

    public GameObject getRobot()
    {
        return robot;
    }


}
