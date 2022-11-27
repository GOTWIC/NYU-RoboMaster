using Mirror;
using System;
using UnityEngine;


[DisallowMultipleComponent]
[AddComponentMenu("Network/Network Manager HUD")]
[RequireComponent(typeof(NetworkManager))]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-manager-hud")]
public class HostControlHud : MonoBehaviour
{
    NetworkManager manager;

    public int offsetX;
    public int offsetY;

    private bool startHostEnabled = false;
    private bool startClientEnabled = false;
    private bool networkAddressInputEnabled = false;

    public static event Action enterRoom;
    public static event Action exitRoom;

    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        // client ready
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            if (GUILayout.Button("Client Ready"))
            {
                NetworkClient.Ready();
                if (NetworkClient.localPlayer == null)
                {
                    NetworkClient.AddPlayer();
                }
            }
        }

        StopButtons();

        GUILayout.EndArea();
    }

    void StartButtons()
    {
        if (!NetworkClient.active)
        {
            // Server + Client
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                startHostEnabled = true;
            }

            // Client + IP

            startClientEnabled = true;


            networkAddressInputEnabled = true;
            // This updates networkAddress every frame from the TextField
            //manager.networkAddress = GUILayout.TextField(manager.networkAddress);


            // FOR NOW, EVERYONE IS EITHER A HOST OR A CLIENT, NO DEDICATED SERVER
            /*
            // Server Only
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // cant be a server in webgl build
                GUILayout.Box("(  WebGL cannot be server  )");
            }
            else
            {
                if (GUILayout.Button("Server Only")) manager.StartServer();
            }
            */
        }
        else
        {
            // Connecting
            GUILayout.Label($"Connecting to {manager.networkAddress}..");
            if (GUILayout.Button("Cancel Connection Attempt"))
            {
                manager.StopClient();
            }
        }
    }

    void StatusLabels()
    {
        // host mode
        // display separately because this always confused people:
        //   Server: ...
        //   Client: ...
        if (NetworkServer.active && NetworkClient.active)
        {
            GUILayout.Label($"<b>Host</b>: running via {Transport.activeTransport}");
        }

        // client only
        else if (NetworkClient.isConnected)
        {
            GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.activeTransport}");
        }
    }

    void StopButtons()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Host"))
            {
                exitRoom?.Invoke();
                manager.StopHost();
            }
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Client"))
            {

                exitRoom?.Invoke();
                manager.StopClient();
            }
        }
    }

    public void startHost()
    {
        if (!startHostEnabled) { return; }

        enterRoom?.Invoke();

        manager.StartHost();
    }

    public void startClient()
    {
        if (!startClientEnabled) { return; }

        enterRoom?.Invoke();

        manager.StartClient();
    }
    
    public void setNetworkAddress(string address)
    {
        if (!networkAddressInputEnabled) { return; }

        manager.networkAddress = address;
    }
}
