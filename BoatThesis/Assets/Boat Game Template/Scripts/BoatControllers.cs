using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GameAnalyticsSDK;

public class BoatControllers : MonoBehaviour {
    //Control types
    public enum ControlTypes {
        Keyboard, Accelerometer,TapControls
    };
    public ControlTypes controlType;
    //boat mesh
    public Transform graphic;
    //mesh's start position
    Vector3 graphicsPosition;
    //motor boat's engine
    public Transform[] rotors;
    //speed of rotors speed
    public float rotorSpeed;
    //Wave variables
    public float waveHeight = 5.0f;
    public float waveFrequency = 2.0f;
    //turning speed
    public float turnSpeed = 5.0f;
    //Max turning angle
    public float maxTurnAngle = 30.0f;
    //controls speed
    public ScrollController speedManager;
    //Game controller
    public GameController gameController;
    //all of the pariticles
    public ParticleSystem speedParticle;
    public ParticleSystem[] floatparticles;
    public ParticleSystem startRipple;
    //current left right input
    float horizontalInput = 0.0f;
    //boat animator
    Animator myAnimator;
    //game over?
    bool isOver = false;
    //started ?
    bool isStarted = false;
    //power up duration
    public float shieldDuration = 2.5f;
    public float gasDuration = 2.5f;
    public float magnetRange = 5.0f;
    public float magnetDuration = 10.0f;
    //power up graphics
    public GameObject shieldGraphic;
    public GameObject magnetGraphic;
    //is shield in use
    bool isProtected = false;
    //coins and gems collected
    int coins = 0;
    int gems = 0;
    //current score
    float score = 0.0f;
    //magnet time controller
    float lastMagnetTime;
    //default boat positions
    float defaultPosition;
    //audio source attached to boat
    AudioSource audioPlayer;
    //all of the sounds
    public AudioClip coinSFX;
    public AudioClip shieldSFX;
    public AudioClip magnetSFX;
    public AudioClip gasSFX;
    public AudioClip crashedSFX;
    public AudioClip reviveSFX;
    public AudioClip shieldOffSFX;
    public AudioClip gemSFX;
    //UI Elements
    public Text coinText;
    public Text gemText;
    public Text scoreText;
    public Text thesisText;
    public string[] thesisWords = { "Sweet", "Good", "Nice", "Positive", "Wow", "Well Done", "Cool", "What a Delight!", "Super" };
    // Use this for initialization setup all the varibales and values
    void Start() {
        //thesisText.text = "";
        Time.timeScale = 1.0f;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        audioPlayer = transform.GetComponent<AudioSource>();

        if (coinText)
            coinText.text = "X " + coins.ToString();
        if (gemText)
            gemText.text = "X " + gems.ToString();
        if (scoreText)
        {
            scoreText.text = "Score : " + score.ToString("F0");
        }

        if (speedManager) {
            speedManager.enabled = false;
        }

        if (graphic)
        {
            graphicsPosition = graphic.position;
        }

        if (gameController) {
            gameController.currentBoat = this;
        }

        foreach (ParticleSystem part in floatparticles)
        {
            if (part)
            {
                part.Stop();
            }
        }
    
        if (speedParticle)
            speedParticle.Stop();
        defaultPosition = transform.position.y;
    }

    //Start the game
    public void StartTheGame() {
        if (startRipple)
            startRipple.Stop();
        foreach (ParticleSystem part in floatparticles)
        {
            if (part)
            {
                part.Play();
            }
        }
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WSAPlayerARM || Application.platform == RuntimePlatform.WSAPlayerX64 || Application.platform == RuntimePlatform.WSAPlayerX86)
        {
            switch (PlayerPrefs.GetInt("ControlType", 0)) {
            
                case 0:
                    controlType = ControlTypes.Accelerometer;
                    break;
                case 1:
                    controlType = ControlTypes.TapControls;
                    break;
            }
        }


        myAnimator = transform.GetComponent<Animator>();

        if (shieldGraphic)
        {
            shieldGraphic.SetActive(false);
        }

        if (speedParticle)
        {
            speedParticle.Stop();
        }

        isStarted = true;

        GameAnalytics.Initialize();
        if (speedManager)
        {
            speedManager.enabled = true;
            InvokeRepeating("AddScore", 1.0f, 1.0f);
            InvokeRepeating("TextMani", 1.0f, 1.0f);
        }
    }

    // Update is called once per frame
    void Update() {
        //Move according to the wave variables
        if (isOver || !isStarted) {
            if (graphic) {
                graphic.position = new Vector3(graphic.position.x,graphicsPosition.y + Mathf.Sin(Time.time * waveFrequency) * waveHeight, graphic.position.z);

            }

            return;
        }
        //Get input based on control type
        switch (controlType) {
            case ControlTypes.Keyboard: horizontalInput = Input.GetAxis("Horizontal"); break;
            case ControlTypes.Accelerometer: horizontalInput = Mathf.Clamp(Input.acceleration.x * 3.0f, -1.0f, 1.0f); break;
            case ControlTypes.TapControls: horizontalInput = TapControlValue(horizontalInput); break;
        }
        //move graphic
        if (graphic)
        {
            graphic.position = new Vector3(graphic.position.x, graphic.position.y + Mathf.Sin((Time.time * waveFrequency)*Time.timeScale) * waveHeight, graphic.position.z);
            transform.eulerAngles = new Vector3(0.0f, Mathf.LerpAngle(transform.eulerAngles.y, maxTurnAngle * horizontalInput, turnSpeed * Time.deltaTime), -20.0f * horizontalInput);
        }
        //rotate rotors
        foreach (Transform rotor in rotors)
        {
            if (rotor)
            {
                rotor.Rotate(Vector3.forward * rotorSpeed);
            }
        }
        //move boat based on input
        transform.position += Vector3.right * (turnSpeed * horizontalInput * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, defaultPosition, transform.position.z);

        //control speed an score
        if (!speedManager)
        {
            Debug.LogWarning("Speed Manager not assigned");
        }

        if (speedManager.holdUpSpeed <= speedManager.maxSpeed)
            speedManager.holdUpSpeed += Time.deltaTime * 0.16f;

        speedManager.speed = speedManager.holdUpSpeed - (Mathf.Abs(horizontalInput) * 5.0f);

        //score += speedManager.speed * Time.deltaTime;

        if (scoreText) {
            scoreText.text = "Score : " + score.ToString("F0"); 
        }

        if (speedManager.speed < 0.0f) {
            speedManager.speed = 0.0f;
        }
        //TextMani();
    }

    public void AddScore()
    {
        score += 1;
    }
    //input based on a type
    float TapControlValue(float oldInput) {
        float xAxis = 0.0f;

        Rect left = new Rect(0, 0, Screen.width / 2.0f, Screen.height);
        Rect right = new Rect(Screen.width / 2.0f, 0, Screen.width / 2.0f, Screen.height);

        foreach (Touch currentTouch in Input.touches) {
            if (left.Contains(currentTouch.position)) {
                xAxis = -1.0f;
                break;
            }
            if (right.Contains(currentTouch.position)){
                xAxis = 1.0f;
                break;
            }

        }

        return Mathf.Lerp(oldInput, xAxis, Time.deltaTime * 5.0f);
        
    }
    //get powerups , see if we collided and other touchy stuff
    void OnTriggerEnter(Collider other) {

        if (isOver)
            return;

        if (other.GetComponent<PowerUps>())
        {
            other.GetComponent<PowerUps>().InitializePowerUp(transform);

            return;
        }else if (isProtected)
        {
            isProtected = false;
            other.enabled = false;
            if (shieldGraphic)
            {
                shieldGraphic.SetActive(false);
            }

            if (audioPlayer && shieldOffSFX)
            {
                audioPlayer.PlayOneShot(shieldOffSFX);
            }

            StopCoroutine(ShieldTimer());
            return;
        }

        Debug.Log("Over");

        AdController.IncrementAdValue();

        if (crashedSFX && audioPlayer)
        {
            audioPlayer.PlayOneShot(crashedSFX);
        }

        Invoke("Over", 1.0f);
        isOver = true;

        foreach (ParticleSystem part in floatparticles)
        {
            if (part)
            {
                part.Stop();
            }
        }

        if (myAnimator) {
            myAnimator.enabled = true;
        }
        Time.timeScale = 1.0f;

        if (magnetGraphic)
        {
            magnetGraphic.SetActive(false);
        }

        StopCoroutine(Magnet());
        if (myAnimator)
            myAnimator.SetTrigger("Over");

        if (speedManager){
            speedManager.enabled = false;
        }
    }

    void Over() {

        if (!gameController) {
            return;
        }
        CancelInvoke();
        string currency = "Gold";
        float amountEarned = score;
        string itemType = "IAP";
        string itemID = "GoldPack";
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, currency, amountEarned, itemType, itemID);
        gameController.SetCoinValue(coins);
        gameController.EnableDisableEndPanel("Over" , score , coins);
        
    }
    //enable shield power up
    public void EnableShield() {
        if (shieldSFX && audioPlayer) {
            audioPlayer.PlayOneShot(shieldSFX);
        }

        StopCoroutine(ShieldTimer());
        isProtected = true;
        if (shieldGraphic)
        {
            shieldGraphic.SetActive(true);
        }
        StartCoroutine(ShieldTimer());

    }

    IEnumerator ShieldTimer() {
        yield return new WaitForSeconds(shieldDuration);
        isProtected = false;
        if (shieldGraphic) {
            shieldGraphic.SetActive(false);
        }
        yield return null;
    }

    public void FillUpGas() {
        Time.timeScale = 1.0f;
        StopCoroutine(SpeedUp());
        StartCoroutine(SpeedUp());
    }
    //speed up power up
    IEnumerator SpeedUp()
    {
        if (gasSFX && audioPlayer)
        {
            audioPlayer.PlayOneShot(gasSFX);
        }

        if (!speedParticle)
            yield return null;
        EnableShield();
        Time.timeScale = 1.5f;
        speedParticle.Play();
        yield return new WaitForSeconds(gasDuration);
        speedParticle.Stop();
        Time.timeScale = 1.0f;
        yield return null;
    }

    //Add coin
    public void AddCoin() {
        if (audioPlayer && coinSFX) {
            audioPlayer.PlayOneShot(coinSFX,0.25f);
        }

        coins++;
        if (coinText)
            coinText.text = "X " + coins.ToString();
    }
    //Add Gem
    public void AddGem()
    {
        if (gemSFX && audioPlayer)
        {
            audioPlayer.PlayOneShot(gemSFX);
        }
        PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems", 0)+1);

        gems++;
        if (gemText)
            gemText.text = "X " + gems.ToString();
    }
    //Enable magnet
    public void EnableMagnet() {
        StopCoroutine(Magnet());
        StartCoroutine(Magnet());
    }

    IEnumerator Magnet() {
        if (magnetSFX && audioPlayer)
        {
            audioPlayer.PlayOneShot(magnetSFX);
        }

        if (magnetGraphic) {
            magnetGraphic.SetActive(true);
        }
        lastMagnetTime = Time.time;
        Collider[] allColliders;
        while ((Time.time - lastMagnetTime) < magnetDuration) {
            allColliders = Physics.OverlapSphere(transform.position, magnetRange);
            foreach (Collider current in allColliders) {
                if (current.isTrigger) {
                    current.SendMessage("SetCoinMagnet", transform,SendMessageOptions.DontRequireReceiver);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (magnetGraphic)
        {
            magnetGraphic.SetActive(false);
        }

        yield return null;
    }
    //revives the boat
    public void Revive() {
        if (!myAnimator) {
            return;
        }

        if (audioPlayer && reviveSFX)
        {
            audioPlayer.PlayOneShot(reviveSFX);
        }

        myAnimator.SetTrigger("Revive");

        StartCoroutine(ReviveRoutine());
    }

    IEnumerator ReviveRoutine() {
        yield return new WaitForSeconds(2.0f);

        EnableShield();
        if (speedManager)
        {
            speedManager.enabled = true;
        }

        foreach (ParticleSystem part in floatparticles)
        {
            if (part)
            {
                part.Play();
            }
        }
        isOver = false;
        yield return null;
    }

    public void TextMani()
    {
        //Debug.Log(score);
        if (score % 10 == 0)
        {
            if (score == 0)
            {
                return;
            }

            thesisText.GetComponent<Text>().text = thesisWords[Random.Range(0, thesisWords.Length)];

        }
        else
        {
            thesisText.GetComponent<Text>().text = "";
        }
        //
    }
    public IEnumerator TextPop()
    {
        
        yield return new WaitForSecondsRealtime(10f);
        
    }
    //Shows magnet area
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRange);
    }
}
