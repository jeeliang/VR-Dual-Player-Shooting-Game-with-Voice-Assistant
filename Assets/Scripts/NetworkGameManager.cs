using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

[System.Serializable]
public struct NamedSpawnPoint
{
    public string name;
    public Transform spawnPoint;

    public string GetName()
    {
        return name;
    }
}

[System.Serializable]
public class Item
{
    public string name;
    public string location;
    public Transform transform;

    public void SetLocation(string l)
    {
        location = l;
        Debug.Log(location);
    }
}

public class NetworkGameManager : NetworkBehaviour
{
    public Transform head;
    public Transform origin;
    public Transform player;
    public Transform tp;
    public Transform[] spawnPositions;
    public Transform curPosition;

    public List<NamedSpawnPoint> spawnPoints; // List of all spawn points
    public List<Item> items; // List of all items to spawn
    public int numberOfItemsToSpawn = 4; // Number of items to spawn
    public int gameRound = 0;
    public int score1;
    public int score2;
    public TextMeshProUGUI player1Score;
    public TextMeshProUGUI player2Score;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI roleText;
    public TextMeshProUGUI winOrLose;
    public string role;
    [SerializeField] float remainingTime = 60f;

    private Dictionary<ulong, bool> playerReadyStatus = new Dictionary<ulong, bool>();
    private bool allPlayersReady = false;

    // Method to shuffle a list
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Returns the location of an item by name
    public string ReturnItemPosition(string itemName)
    {
        for (int i = 0; i < numberOfItemsToSpawn; i++)
        {
            if (items[i].name == itemName)
                return items[i].location;
        }
        return "not found";
    }

    // Sets the position of items
    public void SetItemsPosition()
    {
        // Shuffle the spawn points and items
        Shuffle(spawnPoints);
        Shuffle(items);

        // Spawn items at the first numberOfItemsToSpawn spawn points
        for (int i = 0; i < numberOfItemsToSpawn; i++)
        {
            Transform sp = spawnPoints[i].spawnPoint;
            items[i].transform.position = sp.position;
            items[i].transform.rotation = sp.rotation;
            items[i].SetLocation(spawnPoints[i].GetName());
            if (items[i].transform.gameObject.CompareTag("Coin") || items[i].transform.gameObject.CompareTag("Portion"))
                items[i].transform.Rotate(Vector3.right, -90f);
        }
    }

    // Sets the position of the player
    public void SetPlayerPosition()
    {
        Vector3 offset = head.position - origin.position;
        offset.y = 0;
        origin.position = player.position - offset;

        int index = gameRound;
        if (IsHost)
        {
            curPosition.position = spawnPositions[index % 2].position;
            curPosition.rotation = spawnPositions[index % 2].rotation;
            player.position = spawnPositions[index % 2].position;
            player.rotation = spawnPositions[index % 2].rotation;
            if (index % 2 == 0)
                role = "Attacker";
            else
                role = "Defender";
        }
        else
        {
            curPosition.position = spawnPositions[(index + 1) % 2].position;
            curPosition.rotation = spawnPositions[(index + 1) % 2].rotation;
            player.position = spawnPositions[(index + 1) % 2].position;
            player.rotation = spawnPositions[(index + 1) % 2].rotation;
            if ((index + 1) % 2 == 0)
                role = "Attacker";
            else
                role = "Defender";
        }
        roleText.text = role;
    }

    // Method to handle defender win scenario
    public void DefenderWin()
    {
        if (IsHost)
        {
            if (role == "Defender")
                NewRound(false);
            else
                NewRound(true);
        }
    }

    // Method to start a new round
    public void NewRound(bool host)
    {
        NewRoundServerRPC(host);
    }

    // Client RPC to start a new round
    [ClientRpc]
    void NewRoundClientRpc(bool host)
    {
        if (gameRound != 0)
        {
            //Determine who is the winner and update current score
            if (IsHost)
            {
                Debug.Log("This is host");
                if (host)
                {
                    Debug.Log("I lose");
                    score2++;
                    player2Score.text = score2.ToString();
                    winOrLose.text = "You Lose!";
                }
                else
                {
                    Debug.Log("I win");
                    score1++;
                    player1Score.text = score1.ToString();
                    winOrLose.text = "You Win!";
                }
            }
            else
            {
                Debug.Log("This is not host");
                if (host)
                {
                    Debug.Log("I win");
                    score1++;
                    player1Score.text = score1.ToString();
                    winOrLose.text = "You Win!";
                }
                else
                {
                    Debug.Log("I lose");
                    score2++;
                    player2Score.text = score2.ToString();
                    winOrLose.text = "You Lose!";
                }
            }
        }

        //Game end if either one of the players won 6 rounds
        if (score1 >= 6 || score2 >= 6)
        {
            ShowItem va = FindAnyObjectByType<ShowItem>();

            if (score1 >= 6)
            {
                winOrLose.text = "Victory!";
                va.GameEnd(true);
            }
            else
            {
                winOrLose.text = "Defeated!";
                va.GameEnd(false);
            }

            StartCoroutine(PauseForSeconds(5f));
        }

        gameRound++;
        Debug.Log("New Round");
        Debug.Log(gameRound);

        StartCoroutine(PauseForSeconds(3f));
    }

    // Method to quit the application
    public void Quit()
    {
        // Log a message to the console (useful for debugging)
        Debug.Log("Application is quitting...");

        // If we are running in a standalone build of the game
#if UNITY_STANDALONE
            Application.Quit();
#endif

        // If we are running in the editor
#if UNITY_EDITOR
        // Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    // Server RPC to start a new round
    [ServerRpc(RequireOwnership = false)]
    void NewRoundServerRPC(bool host)
    {
        NewRoundClientRpc(host);
    }

    // Method to update player's HP
    public void ReduceHP(int hp)
    {
        hpText.text = hp.ToString();
    }

    // Coroutine to pause the game for a specified duration when new round start
    IEnumerator PauseForSeconds(float duration)
    {
        // Pause the scene
        Time.timeScale = 0f;
        winOrLose.gameObject.SetActive(true);

        ShowItem va = FindAnyObjectByType<ShowItem>();
        va.ResetCommand();

        PlayerProperties players = FindAnyObjectByType<PlayerProperties>();
        players.ResetHP();
        SetItemsPosition();
        SetPlayerPosition();

        remainingTime = 60;

        // Wait for the specified duration
        yield return new WaitForSecondsRealtime(duration);

        // Resume the scene
        winOrLose.gameObject.SetActive(false);
        Time.timeScale = 1f;

        if (duration == 5)
            Quit();
    }

    // Server RPC to mark player as ready
    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadyServerRPC(ulong clientId)
    {
        playerReadyStatus[clientId] = true;
        CheckAllPlayersReady();
    }

    // Method to check if all players are ready
    private void CheckAllPlayersReady()
    {
        foreach (var playerReady in playerReadyStatus.Values)
        {
            if (!playerReady)
                return;
        }

        allPlayersReady = true;
        NewRound(true);
    }

    // Called when the network object is spawned
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                playerReadyStatus[client.ClientId] = false;
            }
        }

        // Notify the server that this client is ready
        PlayerReadyServerRPC(NetworkManager.Singleton.LocalClientId);
    }

    // Start is called before the first frame update
    void Start()
    {
        winOrLose.gameObject.SetActive(false);
        int index = gameRound;
        if (IsHost)
        {
            curPosition.position = spawnPositions[index % 2].position;
            curPosition.rotation = spawnPositions[index % 2].rotation;
        }
        else
        {
            curPosition.position = spawnPositions[(index + 1) % 2].position;
            curPosition.rotation = spawnPositions[(index + 1) % 2].rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime < 0)
        {
            remainingTime = 0;
            DefenderWin();
        }
        timer.text = remainingTime.ToString("0.00");
    }
}
