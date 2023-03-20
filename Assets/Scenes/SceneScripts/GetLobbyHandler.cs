using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GetLobbyHandler : MonoBehaviour
{
    public GameObject entryPrefab;
    public GameObject entryContainer;
    public TMP_InputField lobbyCode;
    public LobbyController lobbyController;

    public async void RefreshLobby()
    {
        try
        {
            var currentLobby = await LobbyService.Instance.GetJoinedLobbiesAsync();
            if (currentLobby.Count > 0)
            {
                var lobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.First());
                lobbyCode.text = lobby.LobbyCode;
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        //Clear existing List
        foreach (Transform child in entryContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //TODO
        //Get List of Players
    }
}
