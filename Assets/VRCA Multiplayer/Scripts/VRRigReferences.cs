using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRigReferences : MonoBehaviour
{
    // Singleton instance of VRRigReferences
    public static VRRigReferences Singleton;

    // References to the VR rig components
    public Transform root;      // Reference to the root transform of the VR rig
    public Transform head;      // Reference to the head transform (typically the HMD)
    public Transform leftHand;  // Reference to the left hand transform
    public Transform rightHand; // Reference to the right hand transform

    private void Awake()
    {
        // Set the singleton instance to this object
        Singleton = this;
    }
}
