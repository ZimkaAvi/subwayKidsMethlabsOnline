using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using kcp2k;
using Mirror.FizzySteam;
using Steamworks;

public class GameNetworkManager : NetworkManager
{
    public static event Action ClientOnConnected;

    [SerializeField] GameObject gameManagerPrefab;

    public List<PlayerNetworked> NetworkPlayers = new List<PlayerNetworked>();

    
    public override void Awake()
    {
        if(MainMenu.UseSteam)
        {
            transport = GetComponent<FizzySteamworks>();
        }
        else
        {
            transport = GetComponent<KcpTransport>(); 
        }

        base.Awake();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        
        GameObject playerInstance = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, playerInstance);

        var player = playerInstance.GetComponent<PlayerNetworked>();
        NetworkPlayers.Add(player);

        
        if(!MainMenu.UseSteam)
            player.DisplayName = $"Player {conn.connectionId}";
        else
        {
            CSteamID steamID = SteamMatchmaking.GetLobbyMemberByIndex(MainMenu.LobbyID, numPlayers - 1);

            player.SteamID = steamID.m_SteamID;

        }
    }

    
    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("Level"))
        {
            GameObject gameManager = Instantiate(gameManagerPrefab);
            NetworkServer.Spawn(gameManager);
        }
    }

    //Срабатывает при отключении игрока
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        var player = conn.identity.GetComponent<PlayerNetworked>();
        NetworkPlayers.Remove(player);
        base.OnServerDisconnect(conn);
    }

    //Срабатывает при выключении сервера
    public override void OnStopServer()
    {
        NetworkPlayers.Clear();
    }

    //Срабатывает на клиенте, при подключении
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        ClientOnConnected?.Invoke();
    }

    //Срабатывает на клиенте, при отключении от сервера
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        SceneManager.LoadScene("Menu");
        Destroy(gameObject);
    }
}
