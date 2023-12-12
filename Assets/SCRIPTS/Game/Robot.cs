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
    [SyncVar]
    [SerializeField] private int robotNumber;

    [SerializeField] List<Collider> ignoredColliders = new List<Collider>();

    [SyncVar] public Vector3 defaultPosition;
    [SyncVar] public Quaternion defaultRotation;

    bool setData = false;

    private void Start()
    {
        defaultPosition = transform.position;
        defaultRotation = transform.rotation;
    }

    private void Update()
    {
        if (setData) { return; }
        
        if(team != -1 && type != -1){
            robotNumber = GameObject.FindWithTag("RefereeSystem").GetComponent<RefereeSystem>().addRobotHealthDisplayLink(team, health, hasAuthority);
            setData = true;
        }
    }


    public void setTeam(int team){
        this.team = team;
    }

    public void setType(int type){
        this.type = type;
    }

    public int getTeam() { return team; }

    [ClientRpc]
    public void resetRobot(Transform transform) {
        Debug.Log("Resetting Robot Position");
        gameObject.transform.position = transform.position;
        gameObject.transform.rotation = transform.rotation;
    }

    [ClientRpc]
    public void resetRobot()
    {
        Debug.Log("Resetting Robot Position");
        gameObject.transform.position = defaultPosition;
        gameObject.transform.rotation = defaultRotation;
    }

    [ClientRpc]
    public void resetRobot(Vector3 position)
    {
        Debug.Log("Resetting Robot Position");
        gameObject.transform.position = position;
        gameObject.transform.rotation = defaultRotation;
    }

}
