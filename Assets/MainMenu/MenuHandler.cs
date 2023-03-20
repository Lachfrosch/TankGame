using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public GameObject StartScreen;
    public GameObject LobbyList;
    public GameObject Lobby;
    public GameObject CreateLobby;

    public enum Menus : int
    {
        StartScreen = 0,
        LobbyList = 1,
        Lobby = 2,
        CreateLobby = 3
    }
    public void SetMenu(int menu)
    {
        StartScreen.SetActive(false);
        LobbyList.SetActive(false);
        Lobby.SetActive(false);
        CreateLobby.SetActive(false);

        switch (menu)
        {
            case (int)Menus.StartScreen:
                StartScreen.SetActive(true);
                break;
            case (int)Menus.LobbyList:
                LobbyList.SetActive(true);
                break;
            case (int)Menus.Lobby:
                Lobby.SetActive(true);
                break;
            case (int)Menus.CreateLobby:
                CreateLobby.SetActive(true);
                break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartScreen.SetActive(true);
        LobbyList.SetActive(false);
        Lobby.SetActive(false);
        CreateLobby.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
