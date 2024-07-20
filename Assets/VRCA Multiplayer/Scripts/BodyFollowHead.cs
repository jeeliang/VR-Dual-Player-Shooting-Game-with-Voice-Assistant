using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyFollowHead : MonoBehaviour
{
    public float verticalOffset; // Offset distance vertically between head and body
    public Transform head; // Reference to the head's transform

    // Update is called once per frame
    void Update()
    {
        // Update position to follow the head with vertical offset
        transform.position = head.position + Vector3.up * verticalOffset;

        // Update rotation to match head's rotation on the y-axis
        transform.eulerAngles = new Vector3(0, head.eulerAngles.y, 0);
    }
}
