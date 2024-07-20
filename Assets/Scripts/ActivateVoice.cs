using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Oculus.Voice;

public class ActivateVoice : MonoBehaviour
{

    public AppVoiceExperience wit;

    private void Start()
    {
        //Initialize the AppVoiceExperience Component when Start
        if (wit == null)
        {
            wit = GetComponentInParent<AppVoiceExperience>();
            Debug.Log("Find wit");
        }
    }

    //Function called when the primary button of the controller is pressed
    public void TriggerPressed(InputAction.CallbackContext context)
    {
        if(context.performed)
            WitActivate();
    }

    //Activate the microphone to capture audio input
    public void WitActivate()
    {
        wit.Activate();
        Debug.Log("activated wit");
    }

}
