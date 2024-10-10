using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] Transform content;
    [SerializeField] LeaderBoardLine linePrefab;

    Dictionary<PlayerNetworked,LeaderBoardLine> lines = new Dictionary<PlayerNetworked,LeaderBoardLine>();


    private void OnEnable()
    {

        List<PlayerNetworked> players = ((GameNetworkManager)NetworkManager.singleton).NetworkPlayers;

        foreach (var player in players)
            CreateLine(player);

      
        StartCoroutine(UpdateLobby());
    }

    private void OnDisable()
    {
    
        foreach(var line in lines.Values)
        {
            line.Remove();
        }
    
        lines.Clear();

    
    }

    private void ClientHandleInfoUpdated(PlayerNetworked player)
    {
        if (lines.ContainsKey(player))
        {
            //���� ����� ����� ���������� � �������
            if (((GameNetworkManager)NetworkManager.singleton).NetworkPlayers.Contains(player))
            {
                //���� ����� ���������� � ������ �������
                //�������� ������
                LeaderBoardLine line = lines[player];
                //��������� ������ ���� ����������
                foreach (var stat in player.Stats)
                    line.UpdateData(stat.Key, stat.Value);
            }
            else
            {
                //������� ������
                lines[player].Remove();
                //�������� ������ �� �������
                lines.Remove(player);
            }
        }
        else
        {
            //���� ������ ��� � ������� - ������� ������
            CreateLine(player);
        }
    }

    private void CreateLine(PlayerNetworked player)
    {
        string name = player.DisplayName;
        int score = player.Stats[PlayerNetworked.E_Stats.Score];
        int deaths = player.Stats[PlayerNetworked.E_Stats.Death];

        var line = Instantiate(linePrefab, content);
        line.Init(name, score, deaths);
        lines.Add(player, line);
    }
    
    IEnumerator UpdateLobby()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            var players = ((GameNetworkManager)NetworkManager.singleton).NetworkPlayers;
            foreach(var player in players) 
                ClientHandleInfoUpdated(player);
        }
    }
}
