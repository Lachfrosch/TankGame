using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class ScoreboardEntryHandler : MonoBehaviour
{
    public TextMeshProUGUI PlayerPlace;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI PlayerScore;
    public TextMeshProUGUI PlayerKills;
    public TextMeshProUGUI PlayerHits;
    public TextMeshProUGUI PlayerDeaths;
    public TextMeshProUGUI PlayerKD;
    private string _PlayerPlace;
    private string _PlayerName;
    private string _PlayerScore;
    private string _PlayerKills;
    private string _PlayerHits;
    private string _PlayerDeaths;
    private string _PlayerKD;

    public void UpdateEntry(string playerPlace, Player player)
    {
        _PlayerPlace = playerPlace;
        _PlayerName = player.Data["PlayerName"].Value;
        _PlayerScore = player.Data["PlayerScore"].Value;
        _PlayerKills = player.Data["PlayerKills"].Value;
        _PlayerHits = player.Data["PlayerHits"].Value;
        _PlayerDeaths = player.Data["PlayerDeaths"].Value;

        float deathsForKD = 1;

        if (_PlayerDeaths != "0")
        {
            deathsForKD = Convert.ToSingle(_PlayerDeaths);
        }

        _PlayerKD = Math.Round(Convert.ToInt32(_PlayerKills) / deathsForKD, 2).ToString(); //Calculating K/D

        PlayerPlace.text = _PlayerPlace;
        PlayerName.text = _PlayerName;
        PlayerScore.text = _PlayerScore;
        PlayerKills.text = _PlayerKills;
        PlayerHits.text = _PlayerHits;
        PlayerDeaths.text = _PlayerDeaths;
        PlayerKD.text = _PlayerKD;
    }
}
