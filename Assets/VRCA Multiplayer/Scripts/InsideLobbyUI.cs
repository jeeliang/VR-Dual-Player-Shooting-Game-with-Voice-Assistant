using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using TMPro;

public class InsideLobbyUI : MonoBehaviour
{
    public UnityEngine.UI.Toggle isReadyToggle;   // Toggle UI element to indicate player's readiness
    public TextMeshProUGUI playersInside;          // Text UI element to display number of players ready

    // Start is called before the first frame update
    void Start()
    {
        isReadyToggle.onValueChanged.AddListener(SetReady);  // Subscribe to toggle value change event
    }

    // Method called when player toggles readiness
    public void SetReady(bool isReady)
    {
        Lobby currentLobby = LobbyManager.Instance.CurrentLobby;  // Get current lobby

        if (currentLobby != null)
        {
            string playerId = AuthenticationService.Instance.PlayerId;  // Get current player's ID
            Player myPlayer = currentLobby.Players.Find(x => x.Id == playerId);  // Find current player in lobby

            if (myPlayer != null)
            {
                if (myPlayer.Data == null)
                {
                    myPlayer.Data = new Dictionary<string, PlayerDataObject>();  // Initialize player data dictionary if null
                }

                // Create a PlayerDataObject indicating player's readiness
                PlayerDataObject isReadyData = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, isReady ? "yes" : "no");

                // Update or add player's readiness status to data dictionary
                if (myPlayer.Data.ContainsKey("isReady"))
                {
                    myPlayer.Data["isReady"] = isReadyData;
                }
                else
                {
                    myPlayer.Data.Add("isReady", isReadyData);
                }

                // Update player data in the lobby
                LobbyManager.Instance.UpdatePlayerData(myPlayer.Data);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        Lobby currentLobby = LobbyManager.Instance.CurrentLobby;  // Get current lobby

        if (currentLobby == null)
        {
            playersInside.text = "0/0";  // Display 0 players if lobby is null
            return;
        }

        int numberOfReady = GetNumberOfReady();  // Get number of players ready
        playersInside.text = numberOfReady + "/" + currentLobby.Players.Count;  // Display number of ready players out of total

        // Check if current player is the host and all players are ready
        if (currentLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            if (numberOfReady == currentLobby.Players.Count)
            {
                LobbyManager.Instance.LockLobby();  // Lock the lobby
                NetworkSceneTransition.Instance.LoadSceneForEverybody(currentLobby.Data["Game Mode"].Value);  // Transition to game scene
            }
        }
    }

    // Method to count number of players who are ready
    public int GetNumberOfReady()
    {
        int numberOfReady = 0;

        Lobby currentLobby = LobbyManager.Instance.CurrentLobby;  // Get current lobby

        foreach (var item in currentLobby.Players)
        {
            // Check if player data exists, has 'isReady' key, and is set to "yes"
            if (item.Data != null && item.Data.ContainsKey("isReady") && item.Data["isReady"].Value == "yes")
                numberOfReady += 1;
        }

        return numberOfReady;  // Return number of ready players
    }
}
