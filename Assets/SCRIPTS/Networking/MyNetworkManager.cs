using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class MyNetworkManager : NetworkRoomManager
{

    [SerializeField] private GameObject standard = null;
    [SerializeField] private GameObject hero = null;

    [SerializeField] List<GameObject> redSpawnPoints = new List<GameObject>();
    [SerializeField] List<GameObject> blueSpawnPoints = new List<GameObject>();

    [SerializeField] private List<GameObject> roomPlayers = new List<GameObject>();
    [SerializeField] private List<GameObject> gamePlayers = new List<GameObject>();

    [SerializeField] private int mapSelection = -1;

    [Scene]
    [SerializeField] public string Scene_3v3;

    [Scene]
    [SerializeField] public string Scene_BobOmb;

    public int redMemberCount = 0;
    public int blueMemberCount = 0;


    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        getSpawnPoints();
    }
    
    public void getSpawnPoints()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");        

        foreach (GameObject spawnPoint in spawnPoints)
        {
            if (spawnPoint.transform.parent.name == "Blue") { blueSpawnPoints.Add(spawnPoint); }
            if (spawnPoint.transform.parent.name == "Red") { redSpawnPoints.Add(spawnPoint); }
        }
    }

    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        // Roomplayer stores basic user options such as team and robot
        // Gameplayer is the player's actual entity inside the simulation (separate from the robot). Currently only used for positioning
        storePlayers(roomPlayer, gamePlayer);
        spawnRobot(conn, roomPlayer, gamePlayer);
        return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
    }

    private void storePlayers(GameObject roomPlayer, GameObject gamePlayer)
    {
        roomPlayers.Add(roomPlayer);
        gamePlayers.Add(gamePlayer);
    }

    private void spawnRobot(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {

        GameObject robot = null;

        RoomPlayer rp = roomPlayer.GetComponent(typeof(RoomPlayer)) as RoomPlayer;

        int robotType = rp.getRobot();
        int teamSelection = rp.getTeam();
        string robotName = rp.getName();

        if(robotType == 0)
        {
            robot = standard;
        }

        else if(robotType == 1)
        {
            robot = hero;
        }


        if (teamSelection == 0)
        {
            gamePlayer.transform.position = redSpawnPoints[redMemberCount].transform.position;
            gamePlayer.transform.rotation = redSpawnPoints[redMemberCount].transform.rotation;
            redMemberCount++;
        }

        else if (teamSelection == 1)
        {
            gamePlayer.transform.position = blueSpawnPoints[blueMemberCount].transform.position;
            gamePlayer.transform.rotation = blueSpawnPoints[blueMemberCount].transform.rotation;
            blueMemberCount++;
        }
        
        // Spawns on Server
        // "conn.identity.transform" is how you get transform of the player object
        GameObject spawnerInstance = Instantiate(robot, gamePlayer.transform.position, gamePlayer.transform.rotation);

        spawnerInstance.GetComponent<Robot>().setTeam(teamSelection);
        spawnerInstance.GetComponent<Robot>().setType(robotType);
        spawnerInstance.GetComponent<Robot>().setName(robotName);

        //Spawns on Network
        NetworkServer.Spawn(spawnerInstance, conn);

        gamePlayer.GetComponent<PlayerScript>().setRobot(spawnerInstance);
    }

    public void resetGame()
    {
        for(int i = 0; i < gamePlayers.Count; i++)
        {
            // Reset Locations
            gamePlayers[i].GetComponent<PlayerScript>().getRobot().GetComponent<Robot>().resetRobot(gamePlayers[i].transform);

            // Reset Robot Health
            gamePlayers[i].GetComponent<PlayerScript>().getRobot().GetComponent<Health>().resetHealth();
            
            // Reset Robot Firing System
            gamePlayers[i].GetComponent<PlayerScript>().getRobot().GetComponent<UnitFiring>().resetFiringSystem();
        }
    }

    
    override public void OnRoomServerPlayersReady() {
        Switch mapSelectionSwitch = GameObject.Find("/UI/MapSelection").GetComponent(typeof(Switch)) as Switch;
        if (mapSelectionSwitch.getIndex() == 0) { GameplayScene = Scene_3v3; }
        else if (mapSelectionSwitch.getIndex() == 1) { GameplayScene = Scene_BobOmb; }
        base.OnRoomServerPlayersReady();
    }  

    public bool nonNetworkedIsServer()
    {
        if(mode == NetworkManagerMode.ClientOnly) { return false; }

        else { return true; }
    }
}
