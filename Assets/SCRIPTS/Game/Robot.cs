using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Robot : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] private int team = -1;
    [SyncVar]
    [SerializeField] private int type = -1;
    [SerializeField] private Health health = null;

    [SerializeField] List<Collider> ignoredColliders = new List<Collider>();

    bool setData = false;

    private void Start()
    {
        foreach (Collider col in ignoredColliders) { Physics.IgnoreCollision(col, GetComponent<Collider>()); }
    }

    private void Update()
    {
        if (setData) { return; }
        
        if(team != -1 && type != -1){
            setData = true;
            GameObject.FindWithTag("RefereeSystem").GetComponent<RefereeSystem>().addRobotHealthDisplayLink(team, health);
        }
    }


    public void setTeam(int team){
        this.team = team;
    }

    public void setType(int type){
        this.type = type;
    }

    public int getTeam() { return team; }
}
