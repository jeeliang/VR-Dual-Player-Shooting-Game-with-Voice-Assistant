using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletHitBody : MonoBehaviour
{
    //Call the bodyHitted function to update the player's health points and destroy the bullet
    public void Hit(GameObject hitby)
    {
        PlayerProperties player = GetComponentInParent<PlayerProperties>();
        if (player)
            player.bodyHitted();

        //Destroy the bullet
        Destroy(hitby);
    }

    //Call the coinEquipped function and translate the position of the item
    public void EquipCoin(GameObject item)
    {
        PlayerProperties player = GetComponentInParent<PlayerProperties>();
        if (player)
            player.coinEquipped();

        item.transform.Translate(0, -50f, 0);
    }

    //Call the potionEquipped function and translate the position of the item
    public void EquipPortion(GameObject item)
    {
        PlayerProperties player = GetComponentInParent<PlayerProperties>();
        if (player)
            player.portionEquipped();

        item.transform.Translate(0, -50f, 0);
    }

    //Call the shieldEquipped function and translate the position of the item
    public void EquipSheild(GameObject item)
    {
        PlayerProperties player = GetComponentInParent<PlayerProperties>();
        if (player)
            player.sheildEquipped();

        item.transform.Translate(0, -50f, 0);
    }

    //Detect the collision happened with the body
    private void OnCollisionEnter(Collision collision)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        if (collision.gameObject.CompareTag("Weapon"))
            Hit(collision.gameObject);
        if (collision.gameObject.CompareTag("Coin"))
            EquipCoin(collision.gameObject);
        if (collision.gameObject.CompareTag("Portion"))
            EquipPortion(collision.gameObject);
        if (collision.gameObject.CompareTag("Sheild"))
            EquipSheild(collision.gameObject);
        Debug.Log("collided");
    }

}
