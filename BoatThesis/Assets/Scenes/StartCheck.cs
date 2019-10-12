using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameAnalyticsSDK;
using UnityEngine.SceneManagement;

public class StartCheck : MonoBehaviour {

    // Use this for initialization
    public InputField[] inputFields;


	void Start () {
        GameAnalytics.Initialize();
     
        if (PlayerPrefs.GetInt("FIRSTTIMEOPENING", 1) == 1)
        {
            Debug.Log("First Time Opening");

            //Set first time opening to false
            PlayerPrefs.SetInt("FIRSTTIMEOPENING", 0);

            foreach (InputField field in inputFields)
            {
                var se = new InputField.SubmitEvent();
                se.AddListener(delegate {
                    SubmitText(field.name, field.text);
                });
                field.onEndEdit = se;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Debug.Log("NOT First Time Opening");

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
	
	public void SubmitText(string namesh, string textesh)
    {
        string armoryType = namesh;
        string itemRarity = textesh;

        string event_id = armoryType + ":" + itemRarity;
        float event_value = 0.0f;
        GameAnalytics.NewDesignEvent(event_id, event_value);
    }
}
