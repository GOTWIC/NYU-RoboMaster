using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{

    [SerializeField] private GameObject standard = null;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        // Spawns on Server
        // "conn.identity.transform is how you get transform of the player object"
        GameObject spawnerInstance = Instantiate(standard, conn.identity.transform.position, conn.identity.transform.rotation);

        //Spawns on Network
        NetworkServer.Spawn(spawnerInstance, conn);

    }
}
