using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Unity.Netcode;
using UnityEngine.Events;

public class LobbyManager : MonoBehaviour
{
    // Singleton instance of LobbyManager
    public static LobbyManager Instance;
    // Timer for sending heartbeat pings to keep the lobby alive
    private float heartBeatTimer = 0;
    // Timer for updating lobby information
    private float updateLobbyTimer = 0;
    // Current lobby instance
    private Lobby currentLobby;

    // UnityEvents for lobby join start and failure
    public UnityEvent OnStartJoinLobby;
    public UnityEvent OnFailedJoinLobby;

    // Flag and data for updating player information
    private bool hasPlayerDataToUpdate = false;
    private Dictionary<string, PlayerDataObject> newPlayerData;

    // Property to get the current lobby
    public Lobby CurrentLobby { get => currentLobby; }

    private void Awake()
    {
        // Set the singleton instance to this object
        Instance = this;
    }

    // Structure to hold lobby data
    public struct LobbyData
    {
        public string lobbyName;
        public int maxPlayer;
        public string gameMode;
    }

    // Asynchronously update player data in the lobby
    public async void UpdatePlayer(Dictionary<string, PlayerDataObject> data)
    {
        UpdatePlayerOptions updateOptions = new UpdatePlayerOptions();
        updateOptions.Data = data;
        currentLobby = await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId, updateOptions);
    }

    // Asynchronously lock the lobby
    public async void LockLobby()
    {
        currentLobby = await Lobbies.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions { IsLocked = true });
    }

    // Asynchronously leave the lobby
    public async void LeaveLobbyAsync()
    {
        // Shut down the network manager if it exists
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // Remove the player from the lobby if it exists
        if (currentLobby != null)
        {
            string id = currentLobby.Id;
            currentLobby = null;
            await Lobbies.Instance.RemovePlayerAsync(id, AuthenticationService.Instance.PlayerId);
        }
    }

    // Update player data and set the flag to update it
    public void UpdatePlayerData(Dictionary<string, PlayerDataObject> data)
    {
        newPlayerData = data;
        hasPlayerDataToUpdate = true;
    }

    // Asynchronously create a lobby
    public async void CreateLobby(LobbyData lobbyData)
    {
        OnStartJoinLobby.Invoke();

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>();

            // Create a relay game and get the join code
            string joinCode = await RelayManager.Instance.CreateRelayGame(lobbyData.maxPlayer);

            // Add join code and game mode to the lobby data
            DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, joinCode);
            lobbyOptions.Data.Add("Join Code Key", dataObject);

            DataObject gameDataObject = new DataObject(DataObject.VisibilityOptions.Public, lobbyData.gameMode);
            lobbyOptions.Data.Add("Game Mode", gameDataObject);

            // Create the lobby with the specified options
            currentLobby = await Lobbies.Instance.CreateLobbyAsync(lobbyData.lobbyName, lobbyData.maxPlayer, lobbyOptions);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            OnFailedJoinLobby.Invoke();
        }
    }

    // Asynchronously quick join a lobby
    public async void QuickJoinLobby()
    {
        OnStartJoinLobby.Invoke();

        try
        {
            // Quick join a lobby
            currentLobby = await Lobbies.Instance.QuickJoinLobbyAsync();
            string relayJoinCode = currentLobby.Data["Join Code Key"].Value;

            // Join the relay game with the obtained join code
            RelayManager.Instance.JoinRelayGame(relayJoinCode);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            OnFailedJoinLobby.Invoke();
        }
    }

    // Asynchronously join a lobby by its ID
    public async void JoinLobby(string lobbyId)
    {
        OnStartJoinLobby.Invoke();

        try
        {
            // Join the lobby by its ID
            currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);
            string relayJoinCode = currentLobby.Data["Join Code Key"].Value;

            // Join the relay game with the obtained join code
            RelayManager.Instance.JoinRelayGame(relayJoinCode);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            OnFailedJoinLobby.Invoke();
        }
    }

    // Update is called once per frame
    async void Update()
    {
        // Send heartbeat pings to keep the lobby alive every 15 seconds
        if (heartBeatTimer > 15)
        {
            heartBeatTimer -= 15;
            if (currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
        }

        heartBeatTimer += Time.deltaTime;

        // Update lobby information every 1.5 seconds
        if (updateLobbyTimer > 1.5f)
        {
            updateLobbyTimer -= 1.5f;
            if (currentLobby != null)
            {
                // Update player data if there is new data to update
                if (hasPlayerDataToUpdate)
                {
                    UpdatePlayer(newPlayerData);
                    hasPlayerDataToUpdate = false;
                }
                else
                {
                    // Otherwise, refresh the current lobby information
                    currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                }
            }
        }

        updateLobbyTimer += Time.deltaTime;
    }
}
