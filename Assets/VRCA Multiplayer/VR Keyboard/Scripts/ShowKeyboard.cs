using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class ShowKeyboard : MonoBehaviour
{
    private TMP_InputField inputField;    // Reference to the TMP_InputField component attached to this GameObject

    // Offset and positioning variables for the keyboard
    public float distance = -0.3f;        // Distance from the position source to place the keyboard
    public float verticalOffset = -0.5f;   // Vertical offset from the position source to place the keyboard

    public Transform positionSource;      // Transform used as the source position for placing the keyboard

    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();    // Get the TMP_InputField component attached to this GameObject
        inputField.onSelect.AddListener(x => OpenKeyboard()); // Add listener to the input field's OnSelect event to open the keyboard
    }

    // Method to open the non-native keyboard
    public void OpenKeyboard()
    {
        // Set the input field for the NonNativeKeyboard instance
        NonNativeKeyboard.Instance.InputField = inputField;
        // Present the keyboard with the current text in the input field
        NonNativeKeyboard.Instance.PresentKeyboard(inputField.text);

        // Calculate the position to place the keyboard based on the position source
        Vector3 direction = positionSource.forward;   // Get forward direction of the position source
        direction.y = 0;    // Ignore vertical component
        direction.Normalize();  // Normalize direction vector

        // Calculate the target position for the keyboard
        Vector3 targetPosition = positionSource.position + direction * distance + Vector3.up * verticalOffset;

        // Reposition the keyboard to the calculated target position
        NonNativeKeyboard.Instance.RepositionKeyboard(targetPosition);

        // Set the caret (cursor) color alpha to 1 (fully opaque)
        SetCaretColorAlpha(1);

        // Subscribe to the OnClosed event of the NonNativeKeyboard instance to handle when the keyboard is closed
        NonNativeKeyboard.Instance.OnClosed += Instance_OnClosed;
    }

    // Event handler for when the keyboard is closed
    private void Instance_OnClosed(object sender, System.EventArgs e)
    {
        // Set the caret (cursor) color alpha to 0 (fully transparent)
        SetCaretColorAlpha(0);
        // Unsubscribe from the OnClosed event to prevent memory leaks
        NonNativeKeyboard.Instance.OnClosed -= Instance_OnClosed;
    }

    // Method to set the alpha value of the caret (cursor) color
    public void SetCaretColorAlpha(float value)
    {
        inputField.customCaretColor = true; // Enable custom caret color
        Color caretColor = inputField.caretColor; // Get current caret color
        caretColor.a = value; // Set alpha value of caret color
        inputField.caretColor = caretColor; // Apply modified caret color to input field
    }
}
