using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyHandler : MonoBehaviour
{
    public LobbyController lobbyController;
    public TMP_InputField lobbyName;
    public TMP_InputField maxPlayers;
    public TMP_Dropdown gameMode;
    public Toggle isPrivate;
    public void CreateLobby()
    {
        lobbyController.CreateLobby(lobbyName.text, Convert.ToInt32(maxPlayers.text), gameMode.options[gameMode.value].text, isPrivate.enabled);
    }
}
