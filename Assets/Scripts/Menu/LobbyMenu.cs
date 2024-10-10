using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyMenu : NetworkBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] TMP_Dropdown levelDropdown;

    [SyncVar(hook = nameof(ClientHandleSelectLevel))]
    int selectedLevel = 0;

    [SerializeField]
    List<string> sceneNames; 


    private void OnEnable()
    {
        //Добавляем уровни в выпадающий список
        levelDropdown.AddOptions(sceneNames);

        //Для хоста
        if (NetworkServer.active && NetworkClient.isConnected) 
        {
            //Активируем кнопки, выпадающий список
            startButton.interactable = true;
            levelDropdown.interactable = true;
        }

    }

    [ServerCallback]
    public void ServerHandleSelectLevel()
    {
        //Меняем номер выбранного уровня
        selectedLevel = levelDropdown.value;
    }

    [ServerCallback]
    public void StartGame()
    {
        //Меняем сцену
        
    }

    [ClientCallback]
    private void ClientHandleSelectLevel(int oldLevel, int newLevel)
    {
        if (!isClientOnly) return;
        levelDropdown.value = newLevel;
    }

}
