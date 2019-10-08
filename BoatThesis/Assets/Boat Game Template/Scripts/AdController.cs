using UnityEngine.Advertisements;

public static class AdController{
    //Show ads after how many deaths
    static int showVideoADAfter = 5; //Deaths
    //counts session deaths
    static int adCounter = 0;

    //Shows skippable ads after a specefied ammount of deaths
	public static void IncrementAdValue(){
        adCounter++;
        if (adCounter >= showVideoADAfter) {
#if UNITY_ADS
            if (Advertisement.IsReady())
            {
                Advertisement.Show();
            }
#endif
            adCounter = 0;
        }
    }
}
