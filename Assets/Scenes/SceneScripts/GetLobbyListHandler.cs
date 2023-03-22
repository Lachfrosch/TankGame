using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;

public class GetLobbyListHandler : MonoBehaviour
{
    public GameObject entryPrefab;
    public GameObject entryContainer;
    public LobbyController lobbyController;
    public MenuHandler menuHandler;
    // Start is called before the first frame update
    void Start()
    {

    }

    public async void GetLobbyList()
    {
        //Clear existing List
        foreach (Transform child in entryContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        //Get List of Lobbies
        var lobbyList = await lobbyController.ListLobbies();

        //Create Entry for each Lobby in List
        for (int i = 0; i < lobbyList.Count; i++)
        {
            Lobby lobby = lobbyList[i];
            //Create New Entry from Prefab
            var entry = Instantiate(entryPrefab, new Vector3(0, i * -75, 0), transform.rotation); //(i * -75) is for vertical spacing

            //Set values
            entry.GetComponent<EntryHandler>().UpdateEntry(menuHandler, lobbyController, lobby.Name, lobby.Id, lobby.Players.Count, lobby.MaxPlayers, lobby.Data["GameMode"].Value);

            //Add to Container
            entry.transform.SetParent(entryContainer.transform, false);

        }
    }
}
