using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerProperties : NetworkBehaviour
{
    private int hp;
    private NetworkGameManager gm;
    private int coins = 0;
    public ShowItem speaker;
    
    // Start is called before the first frame update
    void Start()
    {
        hp = 100;
        gm = FindObjectOfType<NetworkGameManager>();
        speaker = FindObjectOfType<ShowItem>();
    }

    //Return the number of coins
    public int ReturnCoin()
    {
        return coins;
    }

    //Client Rpc function when the player get headshot
    [ClientRpc]
    void HeadHittedClientRpc()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        hp -= 100;
        gm.ReduceHP(hp);
        Debug.Log("Head Hitted");
    }

    //Client Rpc function when the player get bodyshot
    [ClientRpc]
    void BodyHittedClientRpc()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        hp -= 40;
        gm.ReduceHP(hp);
        Debug.Log("Body Hitted");
    }

    //Function to call the client Rpc when the player get bodyshot
    public void bodyHitted(){
        BodyHittedClientRpc();
    }

    //Function to call the client Rpc when the player get headshot
    public void headHitted(){
        HeadHittedClientRpc();
    }

    //Client Rpc function when the player collect a coin
    [ClientRpc]
    void CoinEquippedClientRpc()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        
        speaker = FindObjectOfType<ShowItem>();
        speaker.IncreaseCoins();
    }

    //Client Rpc function when the player collect a potion
    [ClientRpc]
    void PortionEquippedClientRpc()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        if (hp < 100)
            hp = 100;
        gm.ReduceHP(hp);
        Debug.Log("Portion equipped");
    }

    //Client Rpc function when the player collect a shield
    [ClientRpc]
    void SheildEquippedClientRpc()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        hp += 50;
        gm.ReduceHP(hp);
        Debug.Log("Sheild equipped");
    }

    //Function to call the client Rpc when the player collect a coin
    public void coinEquipped()
    {
        CoinEquippedClientRpc();
    }

    //Function to call the client Rpc when the player collect a potion
    public void portionEquipped()
    {
        PortionEquippedClientRpc();
    }

    //Function to call the client Rpc when the player collect a shield
    public void sheildEquipped()
    {
        SheildEquippedClientRpc();
    }

    //Function to reset health point and coins in new round
    public void ResetHP()
    {
        hp = 100;
        gm.ReduceHP(hp);
        coins = 0;
    }
    
    //Reset the position of player in new round
    public void ResetPosition(Transform curPosition)
    {
        Transform t = GetComponentInParent<Transform>();
        if (curPosition == null)
            return;
        t.position = curPosition.position;
        t.rotation = curPosition.rotation;
        Debug.Log(t.position);
    }

    //Function called when the player is dead
    public void Death(){
        hp = 100;
        gm.ReduceHP(hp);
        coins = 0;
        gm.NewRound(IsHost);
    }

    // Update is called once per frame
    void Update()
    {
        //Check if health point of player less or equal than 0 to call Death() function
        if (hp <= 0){
            Death();
        }

        if (gm == null)
        {
            Debug.Log("FIndGM");
            gm = FindObjectOfType<NetworkGameManager>();
        }
    }
}
