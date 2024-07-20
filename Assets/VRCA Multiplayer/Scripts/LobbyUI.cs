using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Netcode;

public class LobbyUI : MonoBehaviour
{
    // UI game objects for different states
    public GameObject authentication;   // Authentication UI
    public GameObject lobbyMenu;        // Main lobby menu UI
    public GameObject createLobby;      // Create lobby UI
    public GameObject lobbyList;        // Lobby list UI
    public GameObject insideLobby;      // Inside lobby UI
    public GameObject loading;          // Loading UI

    // Buttons for UI interaction
    public Button quickJoinButton;      // Button to quick join a lobby
    public Button createLobbyButton;    // Button to show create lobby UI
    public Button lobbyListButton;      // Button to show lobby list UI

    // Method to enable specific UI elements based on index
    public void UIEnabler(int index)
    {
        GameObject[] uiElements = new GameObject[] { lobbyMenu, createLobby, lobbyList, authentication, insideLobby, loading };

        for (int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].SetActive(i == index);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize UI by showing the lobby menu
        UIEnabler(0);

        // Subscribe to the SignedIn event of AuthenticationService to show lobby menu after signing in
        AuthenticationService.Instance.SignedIn += () => UIEnabler(0);

        // Set up button click listeners
        quickJoinButton.onClick.AddListener(() => LobbyManager.Instance.QuickJoinLobby());
        createLobbyButton.onClick.AddListener(() => UIEnabler(1));
        lobbyListButton.onClick.AddListener(() => UIEnabler(2));

        // Subscribe to network callbacks
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        // Subscribe to events from LobbyManager to handle UI changes during lobby operations
        LobbyManager.Instance.OnStartJoinLobby.AddListener(() => UIEnabler(5));  // Show loading UI when joining/creating lobby
        LobbyManager.Instance.OnFailedJoinLobby.AddListener(() => UIEnabler(0)); // Show lobby menu if join/create lobby fails
    }

    // Method to handle quick join button click
    public void QuickJoin()
    {
        LobbyManager.Instance.QuickJoinLobby();
    }

    // Callback when a client disconnects from the server
    private void OnClientDisconnectCallback(ulong clientId)
    {
        // Check if the disconnected client is not the server itself
        if (!NetworkManager.Singleton.IsServer)
        {
            // Reset UI to lobby menu and leave the lobby asynchronously
            LeaveLobbyUI();
        }
    }

    // Method to leave the lobby and reset UI to lobby menu
    public void LeaveLobbyUI()
    {
        UIEnabler(0);                       // Show lobby menu UI
        LobbyManager.Instance.LeaveLobbyAsync(); // Leave the lobby asynchronously using LobbyManager
    }

    // Callback when a client successfully connects to the server
    private void OnClientConnected(ulong clientId)
    {
        // Check if the connected client is the local client
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            UIEnabler(4);   // Show inside lobby UI for the local client
        }
    }

}
