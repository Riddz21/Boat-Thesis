using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LogoScene : MonoBehaviour {
    //Start after this amount of time
    public float timeToLoad = 1.5f;
	// Use this for initialization
	void Start () {
        Invoke("LoadGame", timeToLoad);

     }
    //Load game
    void LoadGame() {
        Initiate.Fade("Game", Color.red, 2.0f);
    }
}
