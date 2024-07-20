using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Meta.WitAi.TTS.Utilities;
using UnityEngine.XR.Interaction.Toolkit;
public class ShowItem : MonoBehaviour
{

    [SerializeField] private UniversalRendererData rendererData = null;
    [SerializeField] public TTSSpeaker speaker;
    public NetworkGameManager gameManager;
    public PlayerProperties playerProperties;
    public ContinuousMoveProviderBase move;
    public bool unlocked = false;
    public int coins = 0;

    private void Start()
    {
        gameManager = FindObjectOfType<NetworkGameManager>();
        playerProperties = FindObjectOfType<PlayerProperties>();
        move = FindObjectOfType<ContinuousMoveProviderBase>();
        move.moveSpeed = 1.5f;
    }

    //Reveal the location of item based on the value return by Wit.ai
    public void showPath(string[] values)
    {
        if (values[0] != "Coin" && values[0] != "AK" && values[0] != "Player" && values[0] != "Portion" && values[0] != "Shield")
        {
            notMatch(values[0]);
            return;
        }
        if (values[0] != "Player")
            SetFeatureActive(out var feature, values[0], true);
        Debug.Log(values[0]);
        VoiceOutput(values[0]);
    }

    //Increase the speed of the defender when special command is used
    public void IncreaseSpeed(string[] values)
    {
        string output;
        if(values[0] != "speed")
        {
            notMatch(values[0]);
            return;
        }

        //Check if special command is unlocked
        if (unlocked)
        {
            //Check if the role of player is defender
            if (gameManager.role == "Defender")
            {
                move.moveSpeed = 2f;
                output = "Sure, your movement speed has been increased for 15 seconds";
                unlocked = false;
            }
            else
                output = "Sorry, you are not Defender.";
        }
        else
            output = "Sorry, you do not have enough coins to unlock special command!";
        
        Debug.Log(values[0]);
        speaker.Speak(output);
        StartCoroutine(ResetSpeed());
    }

    //Reset the speed of defender after 15 seconds
    public IEnumerator ResetSpeed()
    {
        yield return new WaitForSeconds(15f);
        move.moveSpeed = 1.5f;
    }

    //Unlock the special command if player has collected 2 coins
    public void UnlockSpecialCommand()
    {
        unlocked = true;
        speaker.Speak("Special Command is Unlocked!");
    }

    //Give the voice output to the player based on command given
    public void VoiceOutput(string keyword)
    {
        string output;
        //If the player use special command
        if (keyword == "Player")
        {
            //Check if special command is unlocked
            if(unlocked)
            {
                //Check if the role of player is attacker
                if (gameManager.role == "Attacker")
                {
                    SetFeatureActive(out var feature, keyword, true);
                    output = "Sure, I will reveal the location of enemy for 15 seconds";
                }
                    
                else
                    output = "Sorry, you are not Attacker.";
            }
            else
                output = "Sorry, you do not have enough coins to unlock special command!";
        }
        else if (keyword == "AK")
        {
            output = "Sure, I will show you the location of AK, the AK is located at the bench.";
        }
        else
        {
            string location = gameManager.ReturnItemPosition(keyword);
            Debug.Log(location);
            output = "Sure, I will show you the location of " + keyword + ", the " + keyword + " is located at " + location;
        }
        speaker.Speak(output);
    }

    //Ask player to repeat the command if command given is not match with available action
    public void notMatch(string value)
    {
        Debug.Log(value);
        speaker.Speak("Sorry, please repeat your command.");
    }

    //Reveal the location of item by setting the Renderer Feature active
    private void SetFeatureActive(out ScriptableRendererFeature feature, string featureName, bool setActive)
    {
        feature = rendererData.rendererFeatures.Where((f) => f.name == featureName).FirstOrDefault();
        feature.SetActive(setActive);
        rendererData.SetDirty();

        StartCoroutine(SetDeactivate(featureName));
    }

    //Set the Renderer Feature deactive after 15 seconds
    public IEnumerator SetDeactivate(string featureName)
    {
        ScriptableRendererFeature feature = rendererData.rendererFeatures.Where((f) => f.name == featureName).FirstOrDefault();

        yield return new WaitForSecondsRealtime(15f);
        feature.SetActive(false);
        rendererData.SetDirty();
    }

    //Increase the number of coins if the player collected a coin
    public void IncreaseCoins()
    {
        coins++;
        Debug.Log("num of coins" + coins);
        //Unlock special command if the player has collected 2 coins
        if (coins == 2)
            UnlockSpecialCommand();
    }

    //Reset when new round start
    public void ResetCommand()
    {
        coins = 0;
        unlocked = false;
        ScriptableRendererFeature feature = rendererData.rendererFeatures.Where((f) => f.name == "Player").FirstOrDefault();
        feature.SetActive(false);
        rendererData.SetDirty();
        feature = rendererData.rendererFeatures.Where((f) => f.name == "AK").FirstOrDefault();
        feature.SetActive(false);
        rendererData.SetDirty();
        feature = rendererData.rendererFeatures.Where((f) => f.name == "Coin").FirstOrDefault();
        feature.SetActive(false);
        rendererData.SetDirty();
        feature = rendererData.rendererFeatures.Where((f) => f.name == "Portion").FirstOrDefault();
        feature.SetActive(false);
        rendererData.SetDirty(); 
        feature = rendererData.rendererFeatures.Where((f) => f.name == "Shield").FirstOrDefault();
        feature.SetActive(false);
        rendererData.SetDirty();
    }

    //Give voice output to the player when the game end
    public void GameEnd(bool win)
    {
        if (win)
            speaker.Speak("Congratulations! You have won the game!");
        else
            speaker.Speak("Unfortunately, you have lost the game!");
    }
}