using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using Steamworks;


public class PlayerNetworked : NetworkBehaviour
{
    //Перечисление для пунктов статистики
    public enum E_Stats
    {
        Score,
        Death
    }

    //Событие для оповещения об изменении информации о клиенте
    public static event Action<PlayerNetworked> ClientOnInfoUpdated;

    //Словарь для хранения статистики( Ключ - значение из перечисления, Значение - целое число)
    public SyncDictionary<E_Stats, int> Stats { get; } = new SyncDictionary<E_Stats, int>();

    //Поле для хранения персонажа
    [SyncVar]
    GameObject character;
    public GameObject Character
    {
        get { return character; }
        [Server]
        set { character = value; }
    }

    //Поле для хранения имени игрока
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    string displayName;
    public string DisplayName
    {
        get { return displayName; }
        [Server]
        set { displayName = value; }
    }


    //Поле для хранения SteamID пользователя

    public override void OnStartServer()
    {
        //Добавление статистики
        Stats.Add(E_Stats.Score, 0);
        Stats.Add(E_Stats.Death, 0);
    }


    void Start()
    {
        //Делаем не удаляемым при переходе между сценами.
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartClient()
    {
        //Подключаем коллбэк на изменение статистики
        Stats.Callback += ClientHandleStatsUpdate;

        //Проверяем, что игрок является только клиентом
        if (!isClientOnly) return;

        //Добавляем игрока в список
        ((GameNetworkManager)NetworkManager.singleton).NetworkPlayers.Add(this);
    }

    public override void OnStopClient()
    {
        //Проверяем, что игрок является только клиентом и удаляем игрока из списка
        if (!isClientOnly)
            ((GameNetworkManager)NetworkManager.singleton).
                NetworkPlayers.Remove(this);

        //Оповещаем об изменении информации
        ClientOnInfoUpdated?.Invoke(this);
    }


    //Срабатывает при изменении словаря со статистикой
    private void ClientHandleStatsUpdate(SyncIDictionary<E_Stats, int>.Operation op, E_Stats key, int item)
    {
        ClientOnInfoUpdated?.Invoke(this);
    }

    //Срабатывает при изменении имени
    void ClientHandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke(this);
    }

}
