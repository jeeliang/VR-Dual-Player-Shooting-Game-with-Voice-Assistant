using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkSceneTransition : MonoBehaviour
{
    // Flag to indicate if a scene is currently loading
    private bool isLoading = false;

    // Singleton instance of the NetworkSceneTransition class
    public static NetworkSceneTransition Instance;

    private void Awake()
    {
        // Set the singleton instance to this object
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to the server started event
        NetworkManager.Singleton.OnServerStarted += ServerStarted;
    }

    // Callback when the server starts
    private void ServerStarted()
    {
        // Subscribe to the scene load complete event
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
    }

    // Callback when the scene load is complete
    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        // Reset the loading flag
        isLoading = false;
    }

    // Method to load a scene for all connected clients
    public void LoadSceneForEverybody(string sceneName)
    {
        // Check if a scene is not already loading
        if (!isLoading)
        {
            // Set the loading flag to true
            isLoading = true;
            // Load the scene for all connected clients
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
