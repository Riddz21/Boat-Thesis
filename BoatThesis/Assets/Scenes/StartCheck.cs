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

            foreach (InputField field in inputFields)
            {
                var xx = field.GetComponent<InputField>();
                var se = new InputField.SubmitEvent();
                se.AddListener(delegate {
                    SubmitText(field.name, field.text);
                });
                xx.onEndEdit = se;
            }
            PlayerPrefs.SetInt("FIRSTTIMEOPENING", 0);


        }
        else
        {
            Debug.Log("NOT First Time Opening");

            SceneManager.LoadScene(Random.Range(2, 5));
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

    public void NextLevel()
    {
        SceneManager.LoadScene(Random.Range(2, 5));
    }
}
