using UnityEngine;
using Mirror;
using System;
using Unity.VisualScripting;

public class Health : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float baseCooldownTime = 5f;

    [SerializeField] private string entityType = "None";
    [SerializeField][SyncVar] private int team = -1;

    [SyncVar(hook = nameof(handleHealthUpdates))]
    [SerializeField] private float currentHealth;

    [SyncVar]
    [SerializeField] private bool isDead = false;

    [SyncVar]
    [SerializeField] private bool isInvulnerable = false;

    [SyncVar]
    [SerializeField] private int numDeaths = 0;

    [SyncVar]
    [SerializeField] private float nextRespawnTime;

    [SyncVar]
    [SerializeField] private float invulnerabilityEndTime = 0;

    [SyncVar]
    [SerializeField] public int timeToRespawn;

    [SerializeField] public GameObject shield;

    [SerializeField] public GameObject lazer1;
    [SerializeField] public GameObject despawn1;
    [SerializeField] public GameObject lazer2;
    [SerializeField] public GameObject despawn2;

    [SerializeField] public Robot robot;
    [SerializeField] public GameObject camera;

    [SerializeField] public bool increaseRespawnTime = true;
    [SerializeField] public bool invulnAfterRespawn = true;

    [SerializeField] public float healthPercentageAfterRespawn = 0.2f;

    private Transform defaultCamTransform;


    public event Action ServerOnRobotDie;
    public event Action ServerOnBaseDie;
    public event Action<float, float> ClientOnHealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    private void teamInitialization()
    {
        if (entityType == "robot")
        {
            team = gameObject.GetComponent<Robot>().getTeam();
        }
    }

    [ServerCallback]
    public void Update()
    {
        // For now, if this script is attached to a base, then don't do anything
        if (entityType == "base") { return; }

        // Invulnerability logic
        if (Time.time > invulnerabilityEndTime) { isInvulnerable = false; }
        else { isInvulnerable = true; }

        manageShield();

        // Robot respawn logic
        // If the robot is dead and the respawn timer has expired, revive the robot, else update respawn timer.
        if (isDead) {
            if(Time.time > nextRespawnTime) {

                resetHealth();
                robot.resetRobot();

                //Debug.Log(transform.position);
                //Debug.Log(camera.transform.position);
                //Debug.Log(camera.transform.localPosition);

                isDead = false;
                currentHealth = maxHealth * healthPercentageAfterRespawn;

                // Invoke Invulnerability
                invulnerabilityEndTime = Time.time + 10f;
            }

            else
            {
                timeToRespawn = (int)(nextRespawnTime - Time.time) + 1;
            }
        }
    }

    [ClientRpc]
    private void manageShield()
    {
        // Don't show shield for self because it blocks view
        if (hasAuthority) { return; }
        if (isInvulnerable && invulnAfterRespawn) { shield.SetActive(true); }
        else { shield.SetActive(false); }
    }

    [Server]
    public void dealDamage(float damage)
    {
        if (currentHealth <= 0) { return; }

        if (isInvulnerable && invulnAfterRespawn) { return; }

        int dmg_multiplier = 1;

        if (entityType == "base") { dmg_multiplier = 2; }

        currentHealth = Mathf.Max(currentHealth - damage * dmg_multiplier, 0);

        if (currentHealth > 0) { return; }

        spawnLazer();
        handleDeath();

        Vector3 heaven = new Vector3(transform.position.x, transform.position.y+10000, transform.position.z);

        // Send to heaven temporarily
        robot.resetRobot(heaven);
        
    }

    [Server]
    public void handleDeath()
    {

        if (entityType == "base") {
            ServerOnBaseDie?.Invoke();
            Debug.Log("Base Died");
            return;
        }


        //Invoke Death Logic on other scripts
        ServerOnRobotDie?.Invoke();

        numDeaths += 1;

        nextRespawnTime = Time.time + baseCooldownTime + (numDeaths-1)*5* Convert.ToInt32(increaseRespawnTime);

        isDead = true;

        Debug.Log("Object was destroyed");

        //defaultCamTransform = camera.transform;
        //Transform currentCamTransform = camera.transform;
        
        
    }

    [Server]
    public void resetHealth()
    {
        currentHealth = maxHealth;
    }

    [ClientRpc]
    public void spawnLazer()
    {
        if (entityType != "base") {
            GameObject lazer_inst = Instantiate(lazer1, new Vector3(transform.position.x, transform.position.y + 250, transform.position.z), lazer1.transform.rotation);
            GameObject despawn_inst = Instantiate(despawn1, new Vector3(transform.position.x, transform.position.y, transform.position.z), despawn1.transform.rotation);
        }

        else
        {
            GameObject lazer_inst = Instantiate(lazer2, new Vector3(transform.position.x, transform.position.y + 250, transform.position.z), lazer2.transform.rotation);
            GameObject despawn_inst = Instantiate(despawn2, new Vector3(transform.position.x, transform.position.y, transform.position.z), despawn2.transform.rotation);
        }
    }

    #endregion

    #region Client

    private void Start()
    {
        teamInitialization();
        shield.SetActive(false);
    }

    private void handleHealthUpdates(float oldHealth, float newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    public bool inDeathState() { return isDead; }

    public int getTimeToRespawn() { return timeToRespawn; }

    public float getCurrentHealth() { return currentHealth; }

    public float getMaxHealth() { return maxHealth; }

    public int getTeam() { return team; }

    #endregion
}