using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor.MemoryProfiler;
using System;

public class MyNetworkManager : NetworkRoomManager
{

    [SerializeField] private GameObject standard = null;

    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        spawnRobot(conn, gamePlayer);
        return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
    }

    private void spawnRobot(NetworkConnectionToClient conn, GameObject spawnPointReference)
    {
        // Spawns on Server
        // "conn.identity.transform is how you get transform of the player object"
        GameObject spawnerInstance = Instantiate(standard, spawnPointReference.transform.position, spawnPointReference.transform.rotation);

        //Spawns on Network
        NetworkServer.Spawn(spawnerInstance, conn);
    }
}
