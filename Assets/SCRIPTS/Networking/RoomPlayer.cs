using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class RoomPlayer : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] private int teamSelection = -1;
    [SyncVar]
    [SerializeField] private int robotSelection = -1;
    [SyncVar]
    [SerializeField] private string playerName = "Player";

    private Switch teamSelectionSwitch = null;
    private Switch robotSelectionSwitch = null;
    private Name nameField = null;


    private void Start()
    {
        if (!hasAuthority) { return; }
        teamSelectionSwitch = GameObject.Find("/UI/TeamSelection").GetComponent(typeof(Switch)) as Switch;
        robotSelectionSwitch = GameObject.Find("/UI/RobotSelection").GetComponent(typeof(Switch)) as Switch;
        nameField = GameObject.Find("/NameObject").GetComponent(typeof(Name)) as Name;
    }


    void Update()
    {
        if (!hasAuthority) { return; }

        teamSelection = teamSelectionSwitch.getIndex();
        robotSelection = robotSelectionSwitch.getIndex();
        playerName = nameField.getPlayerName();

        updateInfo(teamSelection, robotSelection, playerName);
    }

    [Command]
    private void updateInfo(int team, int robot, string name)
    {
        teamSelection = team;
        robotSelection = robot;
        playerName = name;
    }

    public int getTeam()
    {
        return teamSelection;
    }

    public int getRobot()
    {
        return robotSelection;
    }

    public string getName()
    {
        return playerName;
    }
}
