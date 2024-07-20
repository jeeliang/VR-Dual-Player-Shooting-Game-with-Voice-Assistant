using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowFPS : MonoBehaviour
{
    public float timer, refresh = 0.5f, avgFrameRate;
    public string display = "FPS {0}";
    public TextMeshProUGUI fps;

    // Update is called once per frame
    void Update()
    {
        // Calculate the time elapsed since the last frame
        float timelapse = Time.smoothDeltaTime;

        // Update the timer, resetting it if it reaches zero or decrementing it by the time elapsed
        timer = timer <= 0 ? refresh : timer -= timelapse;

        // If the timer has reached zero, update the average frame rate
        if (timer <= 0)
        {
            avgFrameRate = (int)(1f / timelapse);
        }

        // Update the FPS display text
        fps.text = string.Format(display, avgFrameRate.ToString());
    }
}
