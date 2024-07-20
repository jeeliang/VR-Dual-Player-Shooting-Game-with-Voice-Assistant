using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class SetOwnershipOnSelect : NetworkBehaviour
{
    // Reference to the XRBaseInteractable component
    private XRBaseInteractable interactable;

    // NetworkVariable to track if the object is currently grabbed over the network
    public NetworkVariable<bool> isNetworkGrabbed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Start is called before the first frame update
    void Start()
    {
        // Get the XRBaseInteractable component attached to this GameObject
        interactable = GetComponent<XRBaseInteractable>();

        // Add listener for the selectEntered event to call SetOwnership when an interaction starts
        interactable.selectEntered.AddListener(x => SetOwnership());

        // Add listener for the selectExited event to call Ungrab when an interaction ends
        interactable.selectExited.AddListener(x => Ungrab());
    }

    // Method to handle setting ownership on selection
    public void SetOwnership()
    {
        // Call the server RPC to set ownership, passing the local client ID
        SetOwnershipServerRPC(NetworkManager.Singleton.LocalClientId);
    }

    // Server RPC to set the ownership of the object
    [ServerRpc(RequireOwnership = false)]
    void SetOwnershipServerRPC(ulong id)
    {
        // Change the ownership of the NetworkObject to the given client ID
        NetworkObject.ChangeOwnership(id);

        // Set the network variable to indicate the object is grabbed
        isNetworkGrabbed.Value = true;
    }

    // Method to handle ungrabbing the object
    public void Ungrab()
    {
        // Call the server RPC to ungrab the object
        UngrabServerRpc();
    }

    // Server RPC to ungrab the object
    [ServerRpc(RequireOwnership = false)]
    public void UngrabServerRpc()
    {
        // Set the network variable to indicate the object is no longer grabbed
        isNetworkGrabbed.Value = false;
    }
}
