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
        //Подключаем коллбэки Steam
        if (!UseSteam) return;

    }

    private void OnDisable()
    {
        if (!UseSteam && lobbyCreated == null) return;
    }

    //Действия при создании лобби
    void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (!UseSteam) return;
        //Проверяет, успешность создания лоби
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            //Выход в оффлайн
            ButtonOffline.GoOffline();
            return;
        }

        //Запоминаем идентификатор лобби

        //Передаем в лобби информацию об идентификаторе хоста

        //Открываем окно лобби
        ShowLobbyMenu();
    }

    //Принятие приглашения или подключение к лобби
    void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        if(!UseSteam) return;
        //Подключение к лобби

    }

    //Срабатывает при подключении к лобби
    void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (!UseSteam) return;
        if (NetworkServer.active) return;

        //Если мененджера сети не существует - создаем
        if(NetworkManager.singleton == null)
            Instantiate(networkManagerPrefab);

        //Получаем идентификатор лобби

        //Читаем информацию об индентификаторе хоста из информации лобби

        //Устанавливаем адрес для подключения

        //Запускаем клиент
        NetworkManager.singleton.StartClient();

        //Прячем окна
        landingPage.SetActive(false);
        joinMenu.SetActive(false);
    }


    public void HostLobby()
    {
        Instantiate(networkManagerPrefab);
        if (UseSteam)
        {
            //Создание лобби
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, NetworkManager.singleton.maxConnections);
        }
        else
        {
            //Переход в режим хоста
            NetworkManager.singleton.StartHost();

            //Открываем окно лобби
            ShowLobbyMenu();
        }
    }

    void ShowLobbyMenu()
    {
        //Прячем окна
        joinMenu.SetActive(false);
        landingPage.SetActive(false);

        //Создаем окно лобби
        GameObject lobbyMenu = Instantiate(lobbyPrefab);
        NetworkServer.Spawn(lobbyMenu);
    }

    //Открывем окно присоединения
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
