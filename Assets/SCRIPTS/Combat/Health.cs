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

    [SerializeField] public GameObject lazer;
    [SerializeField] public GameObject despawn;

    [SerializeField] public bool increaseRespawnTime = true;


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
                isDead = false;
                currentHealth = maxHealth * 0.2f;

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
        if (isInvulnerable) { shield.SetActive(true); }
        else { shield.SetActive(false); }
    }

    [Server]
    public void dealDamage(float damage)
    {
        if (currentHealth <= 0) { return; }

        if (isInvulnerable) { return; }

        int dmg_multiplier = 1;

        if (entityType == "base") { dmg_multiplier = 2; }

        currentHealth = Mathf.Max(currentHealth - damage * dmg_multiplier, 0);

        if (currentHealth > 0) { return; }

        spawnLazer();
        handleDeath();

        
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
    }

    [Server]
    public void resetHealth()
    {
        currentHealth = maxHealth;
    }

    [ClientRpc]
    public void spawnLazer()
    {
        GameObject lazer_inst = Instantiate(lazer, new Vector3(transform.position.x, transform.position.y+100, transform.position.z), lazer.transform.rotation);
        GameObject despawn_inst = Instantiate(despawn, new Vector3(transform.position.x, transform.position.y, transform.position.z), despawn.transform.rotation);
        //NetworkServer.Spawn(lazer_inst);
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