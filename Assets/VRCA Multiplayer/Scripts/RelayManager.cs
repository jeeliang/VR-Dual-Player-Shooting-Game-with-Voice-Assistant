using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    // Singleton instance of RelayManager
    public static RelayManager Instance;
    // Reference to the UnityTransport component
    private UnityTransport transport;

    private void Awake()
    {
        // Set the singleton instance to this object
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get the UnityTransport component attached to this GameObject
        transport = GetComponent<UnityTransport>();
    }

    // Asynchronously create a relay game with a specified maximum number of players
    public async Task<string> CreateRelayGame(int maxPlayer)
    {
        // Create a relay allocation for the specified number of players
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayer);

        // Get the join code for the created relay allocation
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        Debug.Log("THE JOIN CODE IS : " + joinCode);

        // Set the host relay data for the UnityTransport component
        transport.SetHostRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );

        // Start the host using the NetworkManager singleton
        NetworkManager.Singleton.StartHost();

        // Return the join code
        return joinCode;
    }

    // Asynchronously join a relay game using a join code
    public async void JoinRelayGame(string joinCode)
    {
        // Join a relay allocation using the join code
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        // Set the client relay data for the UnityTransport component
        transport.SetClientRelayData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData
        );

        // Start the client using the NetworkManager singleton
        NetworkManager.Singleton.StartClient();
    }
}
