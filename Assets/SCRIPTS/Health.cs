using UnityEngine;
using Mirror;
using System;
using System.Diagnostics.Contracts;

public class Health : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float baseCooldownTime = 5f;



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
    [SerializeField] public int timeToRespawn;


    public event Action ServerOnDie;
    public event Action<float, float> ClientOnHealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    [ServerCallback]
    public void Update()
    {
        // If the robot is dead and the respawn timer has expired, revive the robot, else update respawn timer.
        if(isDead)
        {
            if(Time.time > nextRespawnTime)
            {
                isDead = false;
                currentHealth = maxHealth * 0.2f;
                // Invoke Invulnerability [NOT IMPLEMENTED YET]
            }

            else
            {
                timeToRespawn = (int)(nextRespawnTime - Time.time) + 1;
            }
        }
    }


    [Server]
    public void dealDamage(float damage)
    {
        if (currentHealth <= 0) { return; }

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (currentHealth > 0) { return; }

        handleDeath();
    }

    [Server]
    public void handleDeath()
    {
        //Invoke Death Logic on other scripts
        ServerOnDie?.Invoke();

        numDeaths += 1;

        nextRespawnTime = Time.time + baseCooldownTime + (numDeaths-1)*5;

        isDead = true;

        Debug.Log("Object was destroyed");
    }

    #endregion

    #region Client

    private void handleHealthUpdates(float oldHealth, float newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    public bool inDeathState()
    {
        return isDead;
    }

    public int getTimeToRespawn()
    {
        return timeToRespawn;
    }

    #endregion
}