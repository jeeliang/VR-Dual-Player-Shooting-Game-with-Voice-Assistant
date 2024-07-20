using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelectionUI : MonoBehaviour
{
    public static AvatarSelectionUI Singleton; // Singleton instance of AvatarSelectionUI

    public Button nextHead; // Button to select next head part
    public Button previousHead; // Button to select previous head part
    public Button nextBody; // Button to select next body part
    public Button previousBody; // Button to select previous body part
    public Slider skinSlider; // Slider for selecting skin color
    public TMPro.TMP_InputField nameInputField; // Input field for entering avatar name

    private NetworkAvatar currentNetworkAvatar; // Reference to the current NetworkAvatar being customized

    // Method to update the index of the head part in avatar customization
    public void UpdateHeadIndex(int newIndex)
    {
        if (!currentNetworkAvatar)
            return;

        // Ensure index wraps around within bounds of head parts array
        if (newIndex >= currentNetworkAvatar.headParts.Length)
            newIndex = 0;
        if (newIndex < 0)
            newIndex = currentNetworkAvatar.headParts.Length - 1;

        // Update network avatar data with new head index
        NetworkAvatar.NetworkAvatarData newdata = currentNetworkAvatar.networkAvatarData.Value;
        newdata.headIndex = newIndex;
        currentNetworkAvatar.networkAvatarData.Value = newdata;
    }

    // Method to update the index of the body part in avatar customization
    public void UpdateBodyIndex(int newIndex)
    {
        if (!currentNetworkAvatar)
            return;

        // Ensure index wraps around within bounds of body parts array
        if (newIndex >= currentNetworkAvatar.bodyParts.Length)
            newIndex = 0;
        if (newIndex < 0)
            newIndex = currentNetworkAvatar.bodyParts.Length - 1;

        // Update network avatar data with new body index
        NetworkAvatar.NetworkAvatarData newdata = currentNetworkAvatar.networkAvatarData.Value;
        newdata.bodyIndex = newIndex;
        currentNetworkAvatar.networkAvatarData.Value = newdata;
    }

    // Method to update the skin color value in avatar customization
    public void UpdateSkinValue(float value)
    {
        if (!currentNetworkAvatar)
            return;

        // Update network avatar data with new skin color value
        NetworkAvatar.NetworkAvatarData newdata = currentNetworkAvatar.networkAvatarData.Value;
        newdata.skinColor = value;
        currentNetworkAvatar.networkAvatarData.Value = newdata;
    }

    // Method to update the name value in avatar customization
    public void UpdateNameValue(string value)
    {
        if (!currentNetworkAvatar)
            return;

        // Update network avatar data with new avatar name
        NetworkAvatar.NetworkAvatarData newdata = currentNetworkAvatar.networkAvatarData.Value;
        newdata.avatarName = value;
        currentNetworkAvatar.networkAvatarData.Value = newdata;
    }

    // Start is called before the first frame update
    void Start()
    {
        Singleton = this; // Set the singleton instance to this script

        // Add listeners for next and previous head buttons
        nextHead.onClick.AddListener(() => UpdateHeadIndex(currentNetworkAvatar.networkAvatarData.Value.headIndex + 1));
        previousHead.onClick.AddListener(() => UpdateHeadIndex(currentNetworkAvatar.networkAvatarData.Value.headIndex - 1));

        // Add listeners for next and previous body buttons
        nextBody.onClick.AddListener(() => UpdateBodyIndex(currentNetworkAvatar.networkAvatarData.Value.bodyIndex + 1));
        previousBody.onClick.AddListener(() => UpdateBodyIndex(currentNetworkAvatar.networkAvatarData.Value.bodyIndex - 1));

        // Add listener for skin slider value changes
        skinSlider.onValueChanged.AddListener(UpdateSkinValue);

        // Add listener for name input field value changes
        nameInputField.onEndEdit.AddListener(UpdateNameValue);
    }

    // Method to initialize the UI with current network avatar data
    public void Initialize(NetworkAvatar networkAvatar)
    {
        currentNetworkAvatar = networkAvatar;

        // Set the name input field text without notifying listeners
        nameInputField.SetTextWithoutNotify(currentNetworkAvatar.networkAvatarData.Value.avatarName.ToString());

        // Set the skin slider value without notifying listeners
        skinSlider.SetValueWithoutNotify(currentNetworkAvatar.networkAvatarData.Value.skinColor);
    }

}
