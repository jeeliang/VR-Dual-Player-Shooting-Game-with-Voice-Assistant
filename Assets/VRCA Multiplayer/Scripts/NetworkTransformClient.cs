using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;

public class NetworkTransformClient : NetworkTransform
{
    // Override the OnIsServerAuthoritative method to specify that the client is authoritative
    protected override bool OnIsServerAuthoritative()
    {
        // Return false to indicate that the server is not authoritative for this transform
        return false;
    }
}

