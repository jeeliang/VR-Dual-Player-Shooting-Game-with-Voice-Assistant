using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletHitHead : MonoBehaviour
{
    //Call the headHitted function to update the player's health points and destroy the bullet
    public void Hit(GameObject hitby)
    {
        PlayerProperties player = GetComponentInParent<PlayerProperties>();
        if (player)
            player.headHitted();

        //Destroy the bullet
        Destroy(hitby);
    }

    //Detect if collision happened with the head
    private void OnCollisionEnter(Collision collision)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        if (collision.gameObject.CompareTag("Weapon"))
            Hit(collision.gameObject);
    }
}
