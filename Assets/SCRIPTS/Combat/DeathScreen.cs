using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class DeathScreen : NetworkBehaviour
{
    [SerializeField] Health health = null;
    [SerializeField] Image background = null;
    [SerializeField] Image crosshair = null;
    [SerializeField] TMP_Text text = null;

    // Start is called before the first frame update
    void Start()
    {
        background.enabled = false;
        text.enabled = false;
        crosshair.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) { return; }

        if (health.inDeathState() == true)
        {
            background.enabled = true;
            text.enabled = true;
            crosshair.enabled = false;

            int timeToRespawn = health.timeToRespawn;
            string s = "s";

            if(timeToRespawn == 1) { s = ""; }

            text.text = "Respawning in " + timeToRespawn.ToString() + " second" + s; 
        }

        else
        {
            background.enabled = false;
            text.enabled = false;
            crosshair.enabled = true;
        }
    }
}
