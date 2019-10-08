using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BoatBuyingSelection : MonoBehaviour {
    //list of all the boats
    public GameObject[] allBoats;
    //gets all the info from all boats
    BoatBuyingInfo[] allBoatInfos;
    //get all the boat positions
    Vector3[] boatPositions;
    //scene camera to move towards the selected boats
    public Transform viewCamera;
    //wave height
    public float waveHeight = 5.0f;
    //wave frequency
    public float waveFrequency = 2.0f;
    //counter variable
    int i = 0;
    //current selected boat
    public int currentSelected = 0;
    //Ui box to show that a boat is locked
    public GameObject isLockedBox;
    //UI Box to buy the boat
    public GameObject isBuyBox;
    //UI Box to show that this boat is current selected boat
    public GameObject isSelectedBox;
    //cost of the boat display
    public Text buyValue;
    //how many coins we have ?
    public Text coinCount;
    //how many gems we have ?
    public Text gemCount;

    // Use this for initialization
    void Start () {
        //currently selected boats
        currentSelected = PlayerPrefs.GetInt("SelectedBoat", 0);

        boatPositions = new Vector3[allBoats.Length];
        allBoatInfos = new BoatBuyingInfo[allBoats.Length];
        //Get infos and positions
        for (int i = 0; i < allBoats.Length; i++) {
            if (allBoats[i]) {
                boatPositions[i] = allBoats[i].transform.localPosition ;
                allBoatInfos[i] = allBoats[i].GetComponent<BoatBuyingInfo>(); 
            }
        }
	}

    // Update is called once per frame
    void Update()
    {
        //Set count values
        if (gemCount && coinCount) {
            gemCount.text = PlayerPrefs.GetInt("Gems", 0) + " X";
            coinCount.text =  PlayerPrefs.GetInt("Coins", 0) + " X";
        }

        //Get all the rendered and turn their color on or off based on selection
        i = 0;
        Renderer[] allRenderers;
        foreach (GameObject boat in allBoats)
        {
            if (boat)
            {
                boat.transform.localPosition = new Vector3(boat.transform.localPosition.x, boatPositions[i].y + Mathf.Sin(Time.time * waveFrequency) * waveHeight, boat.transform.localPosition.z);
                allRenderers = allBoats[i].GetComponentsInChildren<Renderer>();
                if (i == currentSelected && viewCamera)
                {
                    viewCamera.transform.position = new Vector3(viewCamera.transform.position.x, viewCamera.transform.position.y, Mathf.Lerp(viewCamera.transform.position.z, allBoats[i].transform.position.z, Time.deltaTime * 5.0f));
                    foreach (Renderer currentRenderer in allRenderers)
                    {
                        currentRenderer.material.color = Color.Lerp(currentRenderer.material.color, Color.white, Time.deltaTime * 5.0f);
                    }
                }
                else {
                    foreach (Renderer currentRenderer in allRenderers)
                    {
                        currentRenderer.material.color = Color.Lerp(currentRenderer.material.color, Color.black, Time.deltaTime * 5.0f);
                    }
                }

                if (allBoatInfos[i] && isLockedBox && isBuyBox && buyValue && isSelectedBox && i == currentSelected)
                {

                    if (!allBoatInfos[i].isLocked)
                    {
                        isLockedBox.SetActive(false);

                        isBuyBox.SetActive(false);

                        isSelectedBox.SetActive(true);
                        
                        if (PlayerPrefs.GetInt("SelectedBoat", 0) != currentSelected)
                            PlayerPrefs.SetInt("SelectedBoat", currentSelected);
                    }
                    else {
                        
                        isLockedBox.SetActive(true);
                        
                        if (allBoatInfos[i].canBuy)
                        {
                            isBuyBox.SetActive(true);
                        }
                        if (buyValue)
                            buyValue.text = "X " + allBoatInfos[i].cost.ToString();

                        isSelectedBox.SetActive(false);

                    }
                }
            }

            i++;

        }
    }

    //Move selection to left
    public void SelectOnLeft() {
        currentSelected--;
        if (currentSelected < 0) {
            currentSelected = allBoats.Length - 1;
        }

        
    }

    //Move selection to right
    public void SelectOnRight()
    {
        currentSelected++;
        if (currentSelected > allBoats.Length-1) {
            currentSelected = 0;
        }

        
    }
    //Go buy the boat
    public void BuyCall() {
        if (allBoatInfos[currentSelected])
            allBoatInfos[currentSelected].BuyTheBoat();
    }
    //Back to the game
    public void Back() {
        Initiate.Fade("Game", Color.white, 2.0f);
    }
}
