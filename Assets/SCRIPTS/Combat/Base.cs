using UnityEngine;
using Mirror;
using System;

public class Base : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] float maxHealth = 2000;

    [SyncVar]
    [SerializeField] float currentHealth;

    [SyncVar]
    [SerializeField] string team;

    [ServerCallback]
    void Start()
    {
        currentHealth = maxHealth;
    }

    [Server]
    void damage(float dmg) {
        currentHealth = Math.Max(0,currentHealth-=dmg);
        if(currentHealth == 0){
            startDeathSequence();
        }
    }

    [Server]
    void startDeathSequence(){
        
    }

    float getHealth(){
        return currentHealth;
    }
}
