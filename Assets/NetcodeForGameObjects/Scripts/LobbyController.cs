using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TMPro;
using System.Linq;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class LobbyController : MonoBehaviour
{
    private Lobby _hostLobby;
    private Lobby _joinedLobby;
    public MenuHandler menuHandler;

    public delegate void UpdateLobbyUI();
    public event UpdateLobbyUI updateLobbyUI;

    private float heartbeatTimer;
    private float heartbeatTimerMax = 15f;
    private float lobbyUpdateTimer;
    private float lobbyUpdateTimerMax = 1.1f;
    private string playerName;

    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as Player: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "Player" + UnityEngine.Random.Range(0, 100);
    }

    // Update is called once per frame
    void Update()
    {
        HandleLobbyheartbeat();
        HandleLobbyPollUpdates();
    }

    private async void HandleLobbyheartbeat()
    {
        if (_hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0f)
            {
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
            }
        }
    }


    private async void HandleLobbyPollUpdates()
    {
        if (_joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer <= 0f)
            {
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                _joinedLobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);


                if (!IsPlayerInLobby())
                {
                    _joinedLobby = null;
                    menuHandler.SetMenu(1);
                }
                else
                {
                    updateLobbyUI?.Invoke();
                }

                if (_joinedLobby == null)
                {
                    menuHandler.SetMenu(1);
                }
            }
        }
    }

    private bool IsPlayerInLobby()
    {
        if (_joinedLobby.Players.Any(x => x.Id == AuthenticationService.Instance.PlayerId))
        {
            return true;
        }
        return false;
    }

    public async void CreateLobby(string lobbyName, int maxPlayers, string gameMode, bool isPrivate)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)}
                }
            };

            _hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            _joinedLobby = _hostLobby;
            menuHandler.SetMenu(2);
            Debug.Log("Created Lobby! " + _hostLobby.Name + " " + _hostLobby.MaxPlayers);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async Task<List<Lobby>> ListLobbies()
    {
        try
        {
            var response = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log("Found " + response.Results.Count + " Lobbies!");
            foreach (var lobby in response.Results)
            {
                Debug.Log(lobby.Name + ": " + lobby.MaxPlayers);
            }
            return response.Results;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
        return null;
    }

    public async void JoinLobbyById(string lobbyId)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };
            _joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, options);
            menuHandler.SetMenu(2);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            _joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
            menuHandler.SetMenu(2);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            menuHandler.SetMenu(1);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void KickPlayer(string playerNameToKick)
    {
        try
        {
            if (playerName != playerNameToKick)
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, _joinedLobby.Players.First(x => x.Data["PlayerName"].Value == playerNameToKick).Id);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
            menuHandler.SetMenu(2);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>{
                { "PlayerName", new PlayerDataObject (PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }

    public Lobby GetCurrentLobby()
    {
        return _joinedLobby;
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_hostLobby.MaxPlayers - 1);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
