using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkXRGrabInteractable : XRGrabInteractable
{
    private SetOwnershipOnSelect selectOwner;

    // Start is called before the first frame update
    void Start()
    {
        // Get the SetOwnershipOnSelect component attached to the same GameObject
        selectOwner = GetComponent<SetOwnershipOnSelect>();
    }

    // Override the IsSelectableBy method to include network ownership logic
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        // Determine if the object is network-grabbable
        // It is grabbable if it is not currently grabbed over the network, or if it is grabbed and the local player is the owner
        bool isNetworkGrabbable = (!selectOwner.isNetworkGrabbed.Value) || (selectOwner.isNetworkGrabbed.Value && selectOwner.IsOwner);

        // Call the base method to check standard XR interaction conditions, and also check the network-grabbable condition
        return base.IsSelectableBy(interactor) && isNetworkGrabbable;
    }
}
