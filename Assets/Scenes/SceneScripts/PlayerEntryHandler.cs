using TMPro;
using UnityEngine;

public class PlayerEntryHandler : MonoBehaviour
{
    public TMP_Text PlayerName;
    private string _PlayerName;
    public GameObject kickIcon;
    private LobbyController _LobbyController;

    public void InitializeEntry(string playerName, LobbyController lobbyController)
    {
        _LobbyController = lobbyController;
        _PlayerName = playerName;
        PlayerName.text = _PlayerName;
        kickIcon.SetActive(lobbyController.IsPlayerHost());        
    }
    public void EntryClicked()
    {
        _LobbyController.KickPlayer(_PlayerName);
    }
}
