using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Discovery
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/Network Discovery HUD")]
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-discovery")]
    [RequireComponent(typeof(NetworkDiscovery))]
    public class MyNetworkDiscoveryHUD : MonoBehaviour
    {
        readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
        Vector2 scrollViewPos = Vector2.zero;

        public NetworkDiscovery networkDiscovery;

        private bool uiReady = false;

        public static event Action enterRoom;
        public static event Action enterServerFinder;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (networkDiscovery == null)
            {
                networkDiscovery = GetComponent<NetworkDiscovery>();
                UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
                UnityEditor.Undo.RecordObjects(new UnityEngine.Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
            }
        }
#endif

        void OnGUI()
        {
            if (NetworkManager.singleton == null)
                return;

            if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
            {
                uiReady = true;
                DrawGUI();
            }


            if (NetworkServer.active || NetworkClient.active)
                StopButtons();
        }

        void StopButtons()
        {
            GUILayout.BeginArea(new Rect(10, 40, 100, 25));

            // stop host if host mode
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                if (GUILayout.Button("Stop Host"))
                {
                    NetworkManager.singleton.StopHost();
                    networkDiscovery.StopDiscovery();
                }
            }
            // stop client if client-only
            else if (NetworkClient.isConnected)
            {
                if (GUILayout.Button("Stop Client"))
                {
                    NetworkManager.singleton.StopClient();
                    networkDiscovery.StopDiscovery();
                }
            }
            // stop server if server-only
            else if (NetworkServer.active)
            {
                if (GUILayout.Button("Stop Server"))
                {
                    NetworkManager.singleton.StopServer();
                    networkDiscovery.StopDiscovery();
                }
            }

            GUILayout.EndArea();
        }

        void Connect(ServerResponse info)
        {
            networkDiscovery.StopDiscovery();
            NetworkManager.singleton.StartClient(info.uri);
        }

        public void OnDiscoveredServer(ServerResponse info)
        {
            // Note that you can check the versioning to decide if you can connect to the server or not using this method
            discoveredServers[info.serverId] = info;
        }

        public void StartHost()
        {
            if (!uiReady) { return; }
            discoveredServers.Clear();
            NetworkManager.singleton.StartHost();
            enterRoom?.Invoke();
            networkDiscovery.AdvertiseServer();
        }

        public void FindServers()
        {
            if (!uiReady) { return; }
            enterServerFinder?.Invoke();
            discoveredServers.Clear();
            networkDiscovery.StartDiscovery();
        }

        public void DrawGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 500));

            // show list of found server

            GUILayout.Label($"Discovered Servers [{discoveredServers.Count}]:");

            // servers
            scrollViewPos = GUILayout.BeginScrollView(scrollViewPos);

            foreach (ServerResponse info in discoveredServers.Values)
                if (GUILayout.Button(info.EndPoint.Address.ToString()))
                {
                    Connect(info);
                    enterRoom?.Invoke();
                }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }


    }
}
