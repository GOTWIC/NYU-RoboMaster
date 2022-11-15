using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(handleHealthUpdates))]
    [SerializeField] private int currentHealth;

    public event Action ServerOnDie;

    public event Action<int, int> ClientOnHealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }


    [Server]
    public void dealDamage(int damage)
    {
        if (currentHealth <= 0) { return; }

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (currentHealth > 0) { return; }

        //Invoke Death Logic on other scripts
        ServerOnDie?.Invoke();

        Debug.Log("Object was destroyed");
    }

    #endregion

    #region Client

    private void handleHealthUpdates(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    #endregion
}