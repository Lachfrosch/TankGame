using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GetLobbyHandler : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject entryContainer;
    public TMP_Text lobbyCode;
    public LobbyController lobbyController;

    public async void RefreshLobby()
    {
        try
        {
            var currentLobbyID = await LobbyService.Instance.GetJoinedLobbiesAsync();
            if (currentLobbyID.Count > 0)
            {
                var lobby = await LobbyService.Instance.GetLobbyAsync(currentLobbyID.First());
                lobbyCode.text = lobby.LobbyCode;

                //Clear existing List
                foreach (Transform child in entryContainer.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                for (int i = 0; i < lobby.Players.Count; i++)
                {
                    Player player = lobby.Players[i];

                    //Create New Entry from Prefab
                    var entry = Instantiate(playerPrefab, new Vector3(0, i * -75, 0), transform.rotation); //(i * -75) is for vertical spacing

                    //Set values
                    entry.GetComponent<PlayerEntryHandler>().InitializeEntry(player.Data["PlayerName"].Value);

                    //Add to Container
                    entry.transform.SetParent(entryContainer.transform, false);
                }
            }


        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
