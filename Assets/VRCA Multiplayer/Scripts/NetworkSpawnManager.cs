using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkSpawnManager : NetworkBehaviour
{
    // Reference to the player Transform
    public Transform player;

    // Array of possible spawn positions
    public Transform[] spawnPositions;

    // Network variable to track the spawn position index, with initial value 0
    private NetworkVariable<int> networkIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Method to set the player's position and rotation to the current spawn position
    public void SetPlayerPosition()
    {
        int index = networkIndex.Value;
        player.position = spawnPositions[index].position;
        player.rotation = spawnPositions[index].rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to the client connected callback
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        // Subscribe to the server started callback
        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
    }

    // Callback when the server starts
    private void Singleton_OnServerStarted()
    {
        if (IsServer)
        {
            // Increment the network index when the server starts
            networkIndex.Value++;
            // Reset the index if it exceeds the number of spawn positions
            if (networkIndex.Value == spawnPositions.Length)
            {
                networkIndex.Value = 0;
            }
        }
    }

    // Callback when a client connects
    private void Singleton_OnClientConnectedCallback(ulong clientID)
    {
        // If the connected client is the local client, set its position
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            SetPlayerPosition();
        }

        if (IsServer)
        {
            // Increment the network index for the server
            networkIndex.Value++;
            // Reset the index if it exceeds the number of spawn positions
            if (networkIndex.Value == spawnPositions.Length)
            {
                networkIndex.Value = 0;
            }
        }
    }
}
