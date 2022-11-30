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

        // Spawns on Server
        // "conn.identity.transform" is how you get transform of the player object
        GameObject spawnerInstance = Instantiate(robot, gamePlayer.transform.position, gamePlayer.transform.rotation);

        //Spawns on Network
        NetworkServer.Spawn(spawnerInstance, conn);
    }
}
