using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameAnalyticsSDK;
using TMPro;
using GameAnalyticsSDK.Events;
#if UNITY_ANDROID || UNITY_IOS || UNITY_WP_8_1
using UnityEngine.Advertisements;
#endif
using UnityEngine.SocialPlatforms;

public class GameController : MonoBehaviour {
    //Color of water
    public Color waterColor = Color.blue;
    //UI Animator
    public Animator uiAnimator;
    //UI Elements
    public Text bestText;
    public Text scoreText;
    public Text coinText;
    public Text gemText;
    public Text gemReviveText;
    
    public Image tiltControlIndicator;
    public Image tapControlIndicator;
    public Image soundBTN;
    public Sprite soundON;
    public Sprite soundOFF;
    public Button gemReviveButton;
    public GameObject revivePanel;
    public GameObject overPanel;
    //Current boat in use
    [HideInInspector]
    public BoatControllers currentBoat;
    //Camera Control
    public SmoothFollow cameraScript;
    //Scroll Script
    public ScrollController scrollController;
    //Coins collected
    int coins = 0;
    float lastTime = 0.0f;
    //times saved counter
    int timesSaved = 0;
    //All the boats
    public GameObject[] allBoats;


    // Use this for initialization
    //Enable selected boat at start

    void Start () {
        int selectedBoat = PlayerPrefs.GetInt("SelectedBoat", 0);
        if (allBoats[selectedBoat]) {
            allBoats[selectedBoat].SetActive(true);
        }
        SetControlVisual();
        SwitchAudioVisual();

    }
	
    //Setup end panel values
    public void EnableDisableEndPanel(string state , float score , int coins){

        if (!scoreText || !bestText || !coinText || !gemText || !revivePanel || !overPanel)
        {
            Debug.LogWarning("Please assign all the variables");
            return;
        }

        if (!uiAnimator)
        {
            Debug.LogWarning("Some variables are not assigned");
            return;
        }

        if (gemReviveText && timesSaved < 3) {
            timesSaved++;
            if (PlayerPrefs.GetInt("Gems", 0) >= (timesSaved * 3)) {
                gemReviveText.text = "Required Gems : " + (timesSaved * 3).ToString() + "\n You have : " + PlayerPrefs.GetInt("Gems", 0).ToString() + " Gems";
                state = "Revive";
            }

            if (timesSaved > 1) {
                if (gemReviveButton) {
                    gemReviveButton.interactable = false;
                }
            }

            
        }

        if (!uiAnimator.enabled)
        {
            uiAnimator.enabled = true;
            return;
        }
        else {
            switch (state) {
                case "Over": uiAnimator.SetTrigger("Over");

                    
                    overPanel.SetActive(true);
                    revivePanel.SetActive(false);
                    AddCoins();
                    
                    break;
                case "Revive": uiAnimator.SetTrigger("Over");
                    overPanel.SetActive(false);
                    revivePanel.SetActive(true);
                    break;
            }
        }



        if (score > PlayerPrefs.GetFloat("Best", 0))
        {
            PlayerPrefs.SetFloat("Best", score);
        }


        bestText.text = "BEST : " + PlayerPrefs.GetFloat("Best", 0).ToString("F0");
        scoreText.text = "SCORE : " + score.ToString("F0");        
        coinText.text = "X " + PlayerPrefs.GetInt("Coins", 0);
        gemText.text = PlayerPrefs.GetInt("Gems", 0) + " X";

    }
    //Set coin value
    public void SetCoinValue(int currentCoins) {
        coins = currentCoins;
    }
    //Add coins
    public void AddCoins() {
        
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + coins);
    }
    //Pause
    public void Pause() {
        lastTime = Time.timeScale;
        Time.timeScale = 0.0f;
    }
    //UnPause
    public void UnPause() {
        Time.timeScale = lastTime;
    }
    //Revive with gems
    public void ReviveWithGems() {
        PlayerPrefs.SetInt("Gems", (PlayerPrefs.GetInt("Gems", 0) - (timesSaved * 3)));
        if (uiAnimator) {
            uiAnimator.SetTrigger("Reset");
        }

        if (currentBoat) {
            currentBoat.Revive();
        }
    }
    //Revive with video
    public void ReviveWithVideo() {
#if UNITY_ADS
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
#endif
    }

#if UNITY_ADS
    //Call back for reward video
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                if (uiAnimator)
                {
                    uiAnimator.SetTrigger("Reset");
                }

                if (currentBoat)
                {
                    currentBoat.Revive();
                }
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
#endif

    
    //Restart game
    public void Restart()
    {
        Initiate.Fade(SceneManager.GetActiveScene().name,waterColor,2.0f);
    }
    //Set Control type button visual
    void SetControlVisual() {
        if (!tiltControlIndicator || !tapControlIndicator) {
            Debug.LogWarning("Please Assign all the variables");
            return;
        }

        switch (PlayerPrefs.GetInt("ControlType", 0))
        {

            case 0:
                tiltControlIndicator.color = Color.green;
                tapControlIndicator.color = Color.black;

                break;
            case 1:
                tiltControlIndicator.color = Color.black;
                tapControlIndicator.color = Color.green;
                break;
        }
    }
    //Get control type
    public void ChangeControlType(int type) {
        switch (type)
        {
            case 0:
                PlayerPrefs.SetInt("ControlType", 1);
                break;
            case 1:
                PlayerPrefs.SetInt("ControlType", 0);
                break;
        }
        SetControlVisual();
    }

    //Switch audio
    public void SwitchAudio() {
        if (PlayerPrefs.GetInt("SFX", 1) == 1) {
            PlayerPrefs.SetInt("SFX", 0);
        } else if (PlayerPrefs.GetInt("SFX", 1) == 0) {
            PlayerPrefs.SetInt("SFX", 1);
        }

        SwitchAudioVisual();
    }

    //Audio toggle button
    public void SwitchAudioVisual() {
        if (!soundBTN || !soundOFF || !soundON)
        {
            Debug.LogWarning("Please Assign all the variables");
            return;
        }

        if (PlayerPrefs.GetInt("SFX", 1) == 1)
        {
            soundBTN.sprite = soundON;
            AudioListener.volume = 1.0f;
        }

        if (PlayerPrefs.GetInt("SFX", 1) == 0)
        {
            soundBTN.sprite = soundOFF;
            AudioListener.volume = 0.0f;
        }
    }

    //start the game
    public void StartTheGame() {
        if (currentBoat) {
            currentBoat.StartTheGame();
            if (cameraScript) {
                cameraScript.target = currentBoat.transform;
            }
        }
        if (scrollController)
        {
            scrollController.StartSetup();
        }
        
    }

    //Go to buying scene
    public void Buy() {
        Initiate.Fade("Buy", Color.white, 2.0f);
    }

    public void Leaderboard() {
        //Call "(int)PlayerPrefs.GetFloat("Best", 0)" to get the best score
        Debug.Log("Leaderboard : Put your code here , Double click to open in IDE");
    }
    //rate button
    public void Rate()
    {
        //Application.OpenURL("Put your URL here then uncomment it");

        Debug.Log("Rate : Put your code here , Double click to open in IDE");
    }
}
