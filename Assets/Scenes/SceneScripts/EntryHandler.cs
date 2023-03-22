using TMPro;
using Unity.Services.Lobbies;
using UnityEngine;

public class EntryHandler : MonoBehaviour
{
    public TextMeshProUGUI LobbyName;
    public TextMeshProUGUI Players;
    public TextMeshProUGUI GameMode;
    private LobbyController _LobbyController;
    private MenuHandler _MenuHandler;
    private string _LobbyId;
    private string _LobbyName;
    private int _CurrentPlayers;
    private int _MaxPlayers;

    public void UpdateEntry(MenuHandler menuHandler, LobbyController lobbycontroller, string lobbyName, string lobbyId, int currentPlayers, int maxPlayers, string gameMode)
    {
        _MenuHandler = menuHandler;
        _LobbyController = lobbycontroller;
        _LobbyName = lobbyName;
        _LobbyId = lobbyId;
        _CurrentPlayers = currentPlayers;
        _MaxPlayers = maxPlayers;
        LobbyName.text = _LobbyName;
        Players.text = _CurrentPlayers + "/" + _MaxPlayers;
        GameMode.text = gameMode;
    }
    public void EntryClicked()
    {
        _LobbyController.JoinLobbyById(_LobbyId);
        _MenuHandler.SetMenu(2);
    }
}
