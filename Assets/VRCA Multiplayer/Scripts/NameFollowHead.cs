using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameFollowHead : MonoBehaviour
{
    public float verticalOffset; // Offset distance vertically between head and name
    public Transform head; // Reference to the head's transform

    private Transform playerCamera; // Reference to the main camera's transform

    private void Start()
    {
        // Get the main camera's transform
        playerCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure the playerCamera is not null
        if (!playerCamera)
            playerCamera = Camera.main.transform;

        // Update position to follow the head with vertical offset
        transform.position = head.position + Vector3.up * verticalOffset;

        // Rotate to face the main camera
        transform.LookAt(playerCamera, Vector3.up);
    }
}
