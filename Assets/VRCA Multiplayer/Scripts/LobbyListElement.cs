using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyListElement : MonoBehaviour
{
    public Button joinButton;
    public TextMeshProUGUI lobbyName;
    public TextMeshProUGUI playerIn;
    public TextMeshProUGUI gameMode;

    private string lobbyId;

    public void Initialize(Lobby lobby)
    {
        lobbyName.text = lobby.Name;
        playerIn.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        gameMode.text = lobby.Data["Game Mode"].Value;
        lobbyId = lobby.Id;
    }

    // Start is called before the first frame update
    void Start()
    {
        joinButton.onClick.AddListener(() => LobbyManager.Instance.JoinLobby(lobbyId));
    }

    public void JoinLobby()
    {
        LobbyManager.Instance.JoinLobby(lobbyId);
        LobbyUI lobbyUI = FindAnyObjectByType<LobbyUI>();
        lobbyUI.UIEnabler(4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
