using UnityEngine;
using Mirror;

public class RoomPlayer : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] private int teamSelection = -1;
    [SyncVar]
    [SerializeField] private int robotSelection = -1;

    private Switch teamSelectionSwitch = null;
    private Switch robotSelectionSwitch = null;


    private void Start()
    {
        if (!hasAuthority) { return; }
        teamSelectionSwitch = GameObject.Find("/UI/TeamSelection").GetComponent(typeof(Switch)) as Switch;
        robotSelectionSwitch = GameObject.Find("/UI/RobotSelection").GetComponent(typeof(Switch)) as Switch;
    }


    void Update()
    {
        if (!hasAuthority) { return; }

        teamSelection = teamSelectionSwitch.getIndex();
        robotSelection = robotSelectionSwitch.getIndex();

        updateInfo(teamSelection, robotSelection);
    }

    [Command]
    private void updateInfo(int team, int robot)
    {
        teamSelection = team;
        robotSelection = robot;
    }

    public int getTeam()
    {
        return teamSelection;
    }

    public int getRobot()
    {
        return robotSelection;
    }
}
