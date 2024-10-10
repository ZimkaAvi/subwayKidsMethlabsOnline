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
        //��������� ������ � ���������� ������
        levelDropdown.AddOptions(sceneNames);

        //��� �����
        if (NetworkServer.active && NetworkClient.isConnected) 
        {
            //���������� ������, ���������� ������
            startButton.interactable = true;
            levelDropdown.interactable = true;
        }

    }

    [ServerCallback]
    public void ServerHandleSelectLevel()
    {
        //������ ����� ���������� ������
        selectedLevel = levelDropdown.value;
    }

    [ServerCallback]
    public void StartGame()
    {
        //������ �����
        
    }

    [ClientCallback]
    private void ClientHandleSelectLevel(int oldLevel, int newLevel)
    {
        if (!isClientOnly) return;
        levelDropdown.value = newLevel;
    }

}
