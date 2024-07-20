using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Gun : NetworkBehaviour
{
    public float speed = 40; // Speed at which the bullet will travel
    public GameObject bullet; // Prefab of the bullet to be fired
    public Transform barrel; // Transform representing the barrel of the gun, where bullets will spawn
    public AudioSource audioSource; // Audio source component for playing the firing sound
    public AudioClip audioClip; // Audio clip to be played when the gun is fired

    // Cooldown variables
    public float fireRate; // Time interval between consecutive shots
    private float nextFireTime = 0f; // Timestamp indicating when the next shot can be fired

    // Method to fire the gun
    public void Fire()
    {
        // Check if enough time has passed since the last shot
        if (Time.time >= nextFireTime)
        {
            FireServerRpc(); // Call the server RPC method to fire the gun
            nextFireTime = Time.time + fireRate; // Update the next allowed fire time
        }
    }

    // Server RPC method to handle firing the gun
    [ServerRpc(RequireOwnership = false)]
    public void FireServerRpc()
    {
        // Instantiate the bullet at the barrel's position and rotation
        GameObject spawnedBullet = Instantiate(bullet, barrel.position, barrel.rotation);

        // Spawn the bullet as a network object
        spawnedBullet.GetComponent<NetworkObject>().Spawn(true);

        // Set the bullet's velocity to make it move forward
        spawnedBullet.GetComponent<Rigidbody>().velocity = speed * barrel.forward;

        // Play the firing sound
        audioSource.PlayOneShot(audioClip);

        // Destroy the bullet after 2 seconds to prevent clutter
        Destroy(spawnedBullet, 2);
    }
}
