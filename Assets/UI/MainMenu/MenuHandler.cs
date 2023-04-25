using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public GameObject MenuCanvas;
    public GameObject StartScreen;
    public GameObject LobbyList;
    public GameObject Lobby;
    public GameObject CreateLobby;
    public GameObject HUDCanvas;
    public GameObject ScoreboardCanvas;
    public GameObject CrosshairZoom;
    public GameObject Crosshair;

    public delegate void UpdateScoreboard();
    public event UpdateScoreboard updateScoreboard;

    private MenuIndex _currentMenuIndex;

    public enum MenuIndex : int
    {
        StartScreen = 0,
        LobbyList = 1,
        Lobby = 2,
        CreateLobby = 3,
        HUD = 4
    }

    public void HideAll()
    {
        MenuCanvas.SetActive(false);
        StartScreen.SetActive(false);
        LobbyList.SetActive(false);
        Lobby.SetActive(false);
        CreateLobby.SetActive(false);
        HUDCanvas.SetActive(false);
        ScoreboardCanvas.SetActive(false);
    }


    public void SetMenu(MenuIndex menu)
    {
        _currentMenuIndex = menu;
        HideAll();
        switch (menu)
        {
            case MenuIndex.StartScreen:
                MenuCanvas.SetActive(true);
                StartScreen.SetActive(true);
                break;
            case MenuIndex.LobbyList:
                MenuCanvas.SetActive(true);
                LobbyList.SetActive(true);
                break;
            case MenuIndex.Lobby:
                MenuCanvas.SetActive(true);
                Lobby.SetActive(true);
                break;
            case MenuIndex.CreateLobby:
                MenuCanvas.SetActive(true);
                CreateLobby.SetActive(true);
                break;
            case MenuIndex.HUD:
                HUDCanvas.SetActive(true);
                break;
        }
    }

    public void PlayButtonClicked()
    {
        SetMenu(MenuIndex.LobbyList);
    }

    public void QuitButtonClicked()
    {
        Application.Quit();
    }

    public void CreateLobbyClicked()
    {
        SetMenu(MenuIndex.CreateLobby);
    }

    public void BackFromLobbyListClicked()
    {
        SetMenu(MenuIndex.StartScreen);
    }

    public void BackFromCreateLobbyClicked()
    {
        SetMenu(MenuIndex.LobbyList);
    }

    public void StartAim()
    {
        CrosshairZoom.SetActive(true);
        Crosshair.SetActive(false);
    }

    public void StopAim()
    {
        CrosshairZoom.SetActive(false);
        Crosshair.SetActive(true);
    }

    public void ShowScoreboard()
    {
        ScoreboardCanvas.SetActive(true);
    }

    public void HideScoreboard()
    {
        ScoreboardCanvas.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartScreen.SetActive(true);
        LobbyList.SetActive(false);
        Lobby.SetActive(false);
        CreateLobby.SetActive(false);
        HUDCanvas.SetActive(false);
        ScoreboardCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideAll();
            switch (_currentMenuIndex)
            {
                case MenuIndex.StartScreen:
                    Application.Quit();
                    break;
                case MenuIndex.LobbyList:
                    MenuCanvas.SetActive(true);
                    StartScreen.SetActive(true);
                    _currentMenuIndex = MenuIndex.StartScreen;
                    break;
                case MenuIndex.Lobby:
                    MenuCanvas.SetActive(true);
                    LobbyList.SetActive(true);
                    _currentMenuIndex = MenuIndex.LobbyList;
                    break;
                case MenuIndex.CreateLobby:
                    MenuCanvas.SetActive(true);
                    LobbyList.SetActive(true);
                    _currentMenuIndex = MenuIndex.LobbyList;
                    break;
                case MenuIndex.HUD:
                    HUDCanvas.SetActive(true);
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab) && _currentMenuIndex == MenuIndex.HUD)
        {
            updateScoreboard?.Invoke();
            ShowScoreboard();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            HideScoreboard();
        }
    }
}
