using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class DeathScreen : NetworkBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Image background;
    [SerializeField] GameObject crosshair;
    [SerializeField] TMP_Text text;
    [SerializeField] RefereeSystem refereeSystem;



    // Start is called before the first frame update
    void Start()
    {
        refereeSystem = GameObject.FindGameObjectWithTag("RefereeSystem").GetComponent<RefereeSystem>();
        background.enabled = false;
        text.enabled = false;
        crosshair.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) { return; }

        if (!refereeSystem.isInPlay()) {
            crosshair.SetActive(false);
        }

        if (health.inDeathState())
        {
            background.enabled = true;
            text.enabled = true;
            crosshair.SetActive(false);

            int timeToRespawn = health.timeToRespawn;
            string s = "s";

            if(timeToRespawn == 1) { s = ""; }

            text.text = "Respawning in " + timeToRespawn.ToString() + " second" + s; 
        }

        else
        {
            background.enabled = false;
            text.enabled = false;
            if (refereeSystem.isInPlay()) { crosshair.SetActive(true); }
        }
    }
}
