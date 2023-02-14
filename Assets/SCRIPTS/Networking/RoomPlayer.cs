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

    [SerializeField] private GameObject frame = null;
    [SerializeField] private GameObject playerCard = null;

    private Switch teamSelectionSwitch = null;
    private Switch robotSelectionSwitch = null;
    private Name nameField = null;

    private NetworkConnection conn = null;


    private void Start()
    {
        if (!hasAuthority) { return; }
        teamSelectionSwitch = GameObject.Find("/UI/TeamSelection").GetComponent(typeof(Switch)) as Switch;
        robotSelectionSwitch = GameObject.Find("/UI/RobotSelection").GetComponent(typeof(Switch)) as Switch;
        nameField = GameObject.Find("/NameObject").GetComponent(typeof(Name)) as Name;

        frame = GameObject.Find("/UI/RoomPlayers/Frame");

        spawnPlayerCard();
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

    [Command]
    private void CMDSpawnPlayerCard()
    {
        conn = GetComponent<NetworkIdentity>().connectionToClient;

        Transform finalSlot = null;

        foreach(Transform slot in frame.transform)
        {
            Debug.Log(slot);
            PlayerCardSlot cardSlotOBJ = slot.GetComponent(typeof(PlayerCardSlot)) as PlayerCardSlot;
            Debug.Log(cardSlotOBJ.getState());
            if (!cardSlotOBJ.getState())
            {
                finalSlot = slot;
                cardSlotOBJ.changeState();
                break;
            }
        }

        /*
        if(finalSlot == null)
        {
            return; // Add Logic here later
        }
        */


        // Spawns on Server
        // "conn.identity.transform" is how you get transform of the player object
        GameObject spawnerInstance = Instantiate(playerCard, finalSlot.position, finalSlot.rotation);

        //Spawns on Network
        NetworkServer.Spawn(spawnerInstance, conn);
    }

    public int getTeam()
    {
        return teamSelection;
    }

    public int getRobot()
    {
        return robotSelection;
    }

    public void spawnPlayerCard()
    {
        CMDSpawnPlayerCard();
    }
}
