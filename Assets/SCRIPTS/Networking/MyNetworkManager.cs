using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor.MemoryProfiler;
using System;

public class MyNetworkManager : NetworkRoomManager
{

    [SerializeField] private GameObject standard = null;
    [SerializeField] private GameObject hero = null;


    [SerializeField] List<GameObject> redSpawnPoints= new List<GameObject>();
    [SerializeField] List<GameObject> blueSpawnPoints = new List<GameObject>();

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

        foreach (GameObject spawnpoint in spawnPoints)
        {
            if (Int32.Parse(spawnpoint.name.Remove(0, 5)) % 2 == 0) { redSpawnPoints.Add(spawnpoint); }
            else { blueSpawnPoints.Add(spawnpoint); }
        }
    }

    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        spawnRobot(conn, roomPlayer, gamePlayer);
        return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
    }

    private void spawnRobot(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {

        GameObject robot = null;

        RoomPlayer rp = roomPlayer.GetComponent(typeof(RoomPlayer)) as RoomPlayer;

        int robotType = rp.getRobot();
        int teamSelection = rp.getTeam();

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

        //Spawns on Network
        NetworkServer.Spawn(spawnerInstance, conn);
    }
}
