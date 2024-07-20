using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateLobbyUI : MonoBehaviour
{
    public TMP_InputField nameInputField;      // Input field for entering lobby name
    public Slider maxPlayerSlider;             // Slider for selecting maximum number of players
    public Button createLobbyButton;           // Button to create the lobby
    public TMP_Dropdown gameModeDropDown;      // Dropdown for selecting game mode

    // Start is called before the first frame update
    void Start()
    {
        // Uncomment this line to add a listener to the createLobbyButton
        // createLobbyButton.onClick.AddListener(CreateLobbyFromUI);
    }

    // Update is called once per frame
    private void Update()
    {
        // Enable createLobbyButton only if nameInputField has text entered
        createLobbyButton.gameObject.SetActive(nameInputField.text != "");
    }

    // Method called when createLobbyButton is clicked
    public void CreateLobbyFromUI()
    {
        // Create lobby data object from UI input
        LobbyManager.LobbyData lobbyData = new LobbyManager.LobbyData();
        lobbyData.maxPlayer = (int)maxPlayerSlider.value;                                      // Set max players from slider value
        lobbyData.lobbyName = nameInputField.text;                                              // Set lobby name from input field
        lobbyData.gameMode = gameModeDropDown.options[gameModeDropDown.value].text;             // Set game mode from dropdown selection

        // Call LobbyManager to create a lobby with the gathered data
        LobbyManager.Instance.CreateLobby(lobbyData);
    }
}
