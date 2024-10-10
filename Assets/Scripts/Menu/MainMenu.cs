using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class MainMenu : MonoBehaviour
{
    public static bool UseSteam { get; private set; } = false;
    [SerializeField] GameObject landingPage, joinMenu, lobbyPrefab, networkManagerPrefab;

    Callback<LobbyCreated_t> lobbyCreated;
    Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    Callback<LobbyEnter_t> lobbyEntered;

    public static CSteamID LobbyID { get; private set; }

    private void OnEnable()
    {
        //���������� �������� Steam
        if (!UseSteam) return;

    }

    private void OnDisable()
    {
        if (!UseSteam && lobbyCreated == null) return;
    }

    //�������� ��� �������� �����
    void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (!UseSteam) return;
        //���������, ���������� �������� ����
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            //����� � �������
            ButtonOffline.GoOffline();
            return;
        }

        //���������� ������������� �����

        //�������� � ����� ���������� �� �������������� �����

        //��������� ���� �����
        ShowLobbyMenu();
    }

    //�������� ����������� ��� ����������� � �����
    void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        if(!UseSteam) return;
        //����������� � �����

    }

    //����������� ��� ����������� � �����
    void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (!UseSteam) return;
        if (NetworkServer.active) return;

        //���� ���������� ���� �� ���������� - �������
        if(NetworkManager.singleton == null)
            Instantiate(networkManagerPrefab);

        //�������� ������������� �����

        //������ ���������� �� ��������������� ����� �� ���������� �����

        //������������� ����� ��� �����������

        //��������� ������
        NetworkManager.singleton.StartClient();

        //������ ����
        landingPage.SetActive(false);
        joinMenu.SetActive(false);
    }


    public void HostLobby()
    {
        Instantiate(networkManagerPrefab);
        if (UseSteam)
        {
            //�������� �����
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, NetworkManager.singleton.maxConnections);
        }
        else
        {
            //������� � ����� �����
            NetworkManager.singleton.StartHost();

            //��������� ���� �����
            ShowLobbyMenu();
        }
    }

    void ShowLobbyMenu()
    {
        //������ ����
        joinMenu.SetActive(false);
        landingPage.SetActive(false);

        //������� ���� �����
        GameObject lobbyMenu = Instantiate(lobbyPrefab);
        NetworkServer.Spawn(lobbyMenu);
    }

    //�������� ���� �������������
    public void ShowJoinMenu()
    {
        Instantiate(networkManagerPrefab);
        joinMenu.SetActive(true);
        landingPage.SetActive(false);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
            Application.Quit();
        #endif
    }
}
