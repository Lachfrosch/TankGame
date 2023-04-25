using TMPro;
using UnityEngine;

public class ScoreboardEntryHandler : MonoBehaviour
{
    public TextMeshProUGUI PlayerPlace;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI PlayerKD;
    private string _PlayerSpot;
    private string _PlayerName;
    private string _PlayerKD;

    public void UpdateEntry(string playerPlace, string playerName, string playerKD)
    {
        _PlayerSpot = playerPlace;
        _PlayerName = playerName;
        _PlayerKD = playerKD;

        PlayerPlace.text = _PlayerSpot;
        PlayerName.text = _PlayerName;
        PlayerKD.text = _PlayerKD;
    }
}
