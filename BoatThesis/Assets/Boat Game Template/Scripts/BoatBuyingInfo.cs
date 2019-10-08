using UnityEngine;
using System.Collections;

public class BoatBuyingInfo : MonoBehaviour {
    //boat order
    public int boatNumber = 0;
    //boat's cost
    public int cost = 5000;
    //do we have enough money to unlock the boat
    public bool isLocked = true;
    //can we buy the boat
    [HideInInspector]
    public bool canBuy = false;
    
    // Use this for initialization
    void Start () {

        AssesInfo();
	}
    //Asses current state of the boat
    public void AssesInfo() {
        isLocked = true;

        if (PlayerPrefs.GetInt("UnlockedBoat" + boatNumber.ToString(), 0) == 1)
        {
            isLocked = false;
        }


        if (boatNumber == 0)
        {
            isLocked = false;
        }

        canBuy = false;

        if (isLocked && PlayerPrefs.GetInt("Coins", 0) >= cost)
        {
            canBuy = true;
        }

    }
    //buy the boat
    public void BuyTheBoat() {
        if (isLocked && canBuy) {
            PlayerPrefs.SetInt("Coins" , PlayerPrefs.GetInt("Coins", 0) - cost);
            PlayerPrefs.SetInt("UnlockedBoat" + boatNumber.ToString(), 1);

        }
        AssesInfo();
    }
}
