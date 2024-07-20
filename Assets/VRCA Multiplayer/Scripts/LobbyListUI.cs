using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class LobbyListUI : MonoBehaviour
{
    public Transform contentParent;             // Parent transform where lobby list elements will be instantiated
    public LobbyListElement lobbyListElementPrefab;  // Prefab of the lobby list element to instantiate
    public float refreshTime = 2;               // Time interval in seconds to refresh the lobby list
    private float timer = 0;                    // Timer to track time for refreshing the lobby list

    // Method to update the lobby list by querying available lobbies
    public async void UpdateLobbyList()
    {
        QueryLobbiesOptions queryOptions = new QueryLobbiesOptions();

        queryOptions.Count = 10;    // Number of lobbies to query
        queryOptions.Order = new List<QueryOrder>();
        QueryOrder byNewOrder = new QueryOrder(false, QueryOrder.FieldOptions.Created); // Order lobbies by creation time

        queryOptions.Order.Add(byNewOrder);

        queryOptions.Filters = new List<QueryFilter>();
        QueryFilter available = new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT); // Filter for lobbies with available slots
        QueryFilter nonLocked = new QueryFilter(QueryFilter.FieldOptions.IsLocked, "0", QueryFilter.OpOptions.EQ); // Filter for unlocked lobbies

        queryOptions.Filters.Add(available);
        queryOptions.Filters.Add(nonLocked);

        QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(queryOptions); // Query lobbies asynchronously

        // Clear existing lobby list elements
        for (int i = 0; i < contentParent.childCount; i++)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }

        // Instantiate new lobby list elements for each queried lobby
        foreach (var lobby in response.Results)
        {
            LobbyListElement spawnElement = Instantiate(lobbyListElementPrefab, contentParent); // Instantiate lobby list element from prefab
            spawnElement.Initialize(lobby); // Initialize the lobby list element with lobby data
        }
    }

    // Method called when this component is enabled (e.g., when the GameObject is activated)
    private void OnEnable()
    {
        UpdateLobbyList(); // Update lobby list when this component is enabled
    }

    // Update is called once per frame
    void Update()
    {
        // Check if it's time to refresh the lobby list
        if (timer > refreshTime)
        {
            UpdateLobbyList(); // Update the lobby list
            timer -= refreshTime; // Reset the timer
        }

        timer += Time.deltaTime; // Increment the timer based on real-time seconds
    }
}
