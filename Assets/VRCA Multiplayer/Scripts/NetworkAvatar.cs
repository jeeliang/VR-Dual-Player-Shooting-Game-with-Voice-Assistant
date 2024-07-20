using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class NetworkAvatar : NetworkBehaviour
{
    // Serialized fields for avatar parts and customization
    public GameObject head;
    public GameObject namePlate;
    public GameObject[] headParts;
    public GameObject[] bodyParts;
    public Renderer[] skinParts;
    public Gradient skinGradient;
    public TMPro.TextMeshPro playerName;

    // Network variable for avatar data
    public NetworkVariable<NetworkAvatarData> networkAvatarData = new NetworkVariable<NetworkAvatarData>(
        new NetworkAvatarData(),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    // Struct for serializing avatar data over the network
    public struct NetworkAvatarData : INetworkSerializable
    {
        public int headIndex;
        public int bodyIndex;
        public float skinColor;
        public FixedString128Bytes avatarName;

        // Serialization method required by INetworkSerializable
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref headIndex);
            serializer.SerializeValue(ref bodyIndex);
            serializer.SerializeValue(ref skinColor);
            serializer.SerializeValue(ref avatarName);
        }
    }

    // Generate random avatar data
    public NetworkAvatarData GenerateRandom()
    {
        int randomHeadIndex = Random.Range(0, headParts.Length);
        int randomBodyIndex = Random.Range(0, bodyParts.Length);
        float randomSkinColor = Random.Range(0f, 1f);

        string avatarName = "Player " + NetworkManager.Singleton.LocalClientId.ToString();

        NetworkAvatarData randomData = new NetworkAvatarData
        {
            headIndex = randomHeadIndex,
            bodyIndex = randomBodyIndex,
            skinColor = randomSkinColor,
            avatarName = avatarName,
        };

        return randomData;
    }

    // Update avatar appearance based on given data
    public void UpdateAvatarFromData(NetworkAvatarData newData)
    {
        // Activate/deactivate head parts based on headIndex
        for (int i = 0; i < headParts.Length; i++)
        {
            headParts[i].SetActive(i == newData.headIndex);
        }

        // Activate/deactivate body parts based on bodyIndex
        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].SetActive(i == newData.bodyIndex);
        }

        // Update skin color of all skin parts based on skinColor using gradient
        foreach (var item in skinParts)
        {
            item.material.color = skinGradient.Evaluate(newData.skinColor);
        }

        // Update player name displayed
        playerName.text = newData.avatarName.ToString();
    }

    // Called when the avatar spawns on the network
    public override void OnNetworkSpawn()
    {
        // Only execute this for the owner client
        if (IsOwner)
        {
            // Generate random avatar data for the owner
            networkAvatarData.Value = GenerateRandom();

            // Set head layer (example: layer 7)
            head.layer = 7;

            // Disable name plate (assuming it's a UI element)
            namePlate.SetActive(false);

            // Initialize avatar selection UI (assuming Singleton is correctly set up)
            AvatarSelectionUI.Singleton.Initialize(this);
        }

        // Update avatar appearance based on current networkAvatarData
        UpdateAvatarFromData(networkAvatarData.Value);

        // Subscribe to changes in networkAvatarData to update appearance dynamically
        networkAvatarData.OnValueChanged += (x, y) => UpdateAvatarFromData(y);
    }
}
