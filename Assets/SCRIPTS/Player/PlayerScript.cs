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
        if (!hasAuthority) { return; }
        
        if (SceneManager.GetActiveScene().name == "Room") { return; }

        if (Input.GetKeyDown(KeyCode.Escape) && !refereeSystem.isEndOfGame()) { togglePause(); }

        if (refereeSystem.isPaused()) {
            Time.timeScale = 0.0f;
        }

        if (refereeSystem.isTransitioning()) {
            Time.timeScale = 0.3f;
        }

        if (refereeSystem.isEndOfGame()) {
            Time.timeScale = 0.0f;
        }

        if (!refereeSystem.isPaused() && !refereeSystem.isTransitioning() && !refereeSystem.isEndOfGame()) {
            Time.timeScale = 1.0f;
        }

        if (Input.GetKeyDown(KeyCode.Delete)) {
            Debug.Log("Quitting");
            Application.Quit();
        }

        
        if (refereeSystem.isEndOfGame()) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        

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

    [Command]
    public void togglePause() {
        refereeSystem.togglePause();
    }
        
        


}
