using TMPro;
using UnityEngine;

public class PlayerEntryHandler : MonoBehaviour
{
    public TMP_InputField LobbyName;
    public TMP_InputField Players;
    public TMP_InputField GameMode;
    private string _LobbyCode;
    private string _LobbyName;
    private int _CurrentPlayers;
    private int _MaxPlayers;

    public void InitializeEntry(string lobbyName, string lobbyCode, int currentPlayers, int maxPlayers)
    {
        _LobbyName = lobbyName;
        _LobbyCode = lobbyCode;
        _CurrentPlayers = currentPlayers;
        _MaxPlayers = maxPlayers;
        LobbyName.text = _LobbyName;
        Players.text = currentPlayers + "/" + maxPlayers;
        //GameMode.text = gameMode;
    }
    public void EntryClicked()
    {

    }
}
