using UnityEngine;
using System.Collections;

public class ChallengeInfo : MonoBehaviour {
    //power up spawm point
    public Transform powerUpPoint;
    //all the powers to be spawned randomly
    public GameObject[] allPowerUps;

    //spawn a power up
    void Start() {
        if (!powerUpPoint) {
            return;
        }

        if (PowerUPCounterClass.CanSpawn()) {
            int rand = Random.Range(0, allPowerUps.Length);
            if (allPowerUps[rand]) {
                GameObject inst = Instantiate(allPowerUps[rand], powerUpPoint.position, Quaternion.identity)as GameObject;
                inst.transform.parent = transform;
            }
        }
    }
    //Shows spawn point
    void OnDrawGizmos()
    {
        if (powerUpPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(powerUpPoint.position, 0.5f);
        }
    }
}
