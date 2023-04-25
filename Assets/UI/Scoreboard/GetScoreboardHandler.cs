using System;
using System.Linq;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GetScoreboardHandler : MonoBehaviour
{
    public GameObject entryPrefab;
    public GameObject entryContainer;
    public LobbyController lobbyController;
    public MenuHandler menuHandler;

    public void RefreshScoreboard()
    {
        //Clear existing List
        foreach (Transform child in entryContainer.transform)
        {
            Destroy(child.gameObject);
        }

        //Get List of Players
        var playerList = lobbyController.ListPlayers();

        playerList.OrderBy(x => (Convert.ToInt32(x.Data["PlayerKills"].Value) / Convert.ToInt32(x.Data["PlayerDeaths"].Value)));

        //Create Entry for each Player in List
        for (int i = 0; i < playerList.Count; i++)
        {
            Player player = playerList[i];
            //Create New Entry from Prefab
            var entry = Instantiate(entryPrefab, new Vector3(0, (i * -75) - 37.5f, 0), transform.rotation); //(i * -75) is for vertical spacing

            //Set values
            entry.GetComponent<ScoreboardEntryHandler>().UpdateEntry((i + 1).ToString(), player.Data["PlayerName"].Value, (Convert.ToInt32(player.Data["PlayerKills"].Value) / Convert.ToInt32(player.Data["PlayerDeaths"].Value)).ToString());

            //Add to Container
            entry.transform.SetParent(entryContainer.transform, false);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        menuHandler.updateScoreboard += RefreshScoreboard;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
