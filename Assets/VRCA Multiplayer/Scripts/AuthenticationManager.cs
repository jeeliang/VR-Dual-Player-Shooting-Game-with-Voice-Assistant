using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class AuthenticationManager : MonoBehaviour
{
    private void Awake()
    {
        // Automatically login when the script awakes
        Login();
    }

    // Asynchronously log in the user
    public async void Login()
    {
        // Create initialization options
        InitializationOptions options = new InitializationOptions();

#if UNITY_EDITOR
        // If running in the Unity Editor and ParrelSync clone, set the profile to the clone argument
        if (ClonesManager.IsClone())
            options.SetProfile(ClonesManager.GetArgument());
        else
            // Otherwise, set the profile to "primary"
            options.SetProfile("primary");
#endif

        // Initialize Unity services with the specified options
        await UnityServices.InitializeAsync(options);
        // Sign in the user anonymously
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
