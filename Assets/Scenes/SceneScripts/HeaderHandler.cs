using TMPro;
using Unity.Services.Lobbies;
using UnityEngine;

public class HeaderHandler : MonoBehaviour
{
    public TextMeshProUGUI LobbyName;
    public TextMeshProUGUI Players;
    public TextMeshProUGUI GameMode;
    private string _LobbyName;
    private int _CurrentPlayers;
    private int _MaxPlayers;

    public void UpdateEntry (string lobbyName, int currentPlayers, int maxPlayers, string gameMode)
    {
        _LobbyName = lobbyName;
        _CurrentPlayers = currentPlayers;
        _MaxPlayers = maxPlayers;
        LobbyName.text = _LobbyName;
        Players.text = _CurrentPlayers + "/" + _MaxPlayers;
        GameMode.text = gameMode;
    }
}
