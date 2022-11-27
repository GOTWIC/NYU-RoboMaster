using UnityEngine;
using Mirror;
using TMPro;

public class DeathScreen : NetworkBehaviour
{
    [SerializeField] Health health = null;
    [SerializeField] Canvas screen = null;
    [SerializeField] TMP_Text text = null;

    // Start is called before the first frame update
    void Start()
    {
        screen.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) { return; }

        if (health.inDeathState() == true)
        {
            screen.enabled = true;
            int timeToRespawn = health.timeToRespawn;
            string s = "s";

            if(timeToRespawn == 1) { s = ""; }

            text.text = "Respawning in " + timeToRespawn.ToString() + " second" + s; 
        }

        else
        {
            screen.enabled = false;
        }
    }
}
