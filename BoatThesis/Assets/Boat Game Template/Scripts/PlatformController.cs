using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour {
    //end point of a panel
    public Transform endPoint;
    //Spawn points for challenges
    public Transform[] challengeSpawnPoints;
    //Challenge holder
    public GameObject challengeHolder;
    //Do not spawn challenges
    public bool skipFirstLoop = false;
    //All the challenges
    GameObject[] challengesArray;

    //Show all the spawn points
    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1.0f);
        Gizmos.color = Color.red;
        if (endPoint) {
            Gizmos.DrawWireSphere(endPoint.position, 1.0f);
        }
        Gizmos.color = Color.blue;

        for (int i = 0; i < challengeSpawnPoints.Length; i++)
        {
            if (challengeSpawnPoints[i] != null)
            {
                Gizmos.DrawWireSphere(challengeSpawnPoints[i].position, 0.5f);
            }
        }

        Gizmos.color = Color.cyan;

    }
    //get challenges array
    public void AssignChallengesArray(GameObject[] arr) {
        challengesArray = arr;
    }
    //get the end point position
    public Vector3 GetEndPoint() {
        if (endPoint)
            return endPoint.position;
        else
            return Vector3.zero;
    }

    //Delete and spawn challenges
    public void CreateChallenge() {
        if (skipFirstLoop) {
            skipFirstLoop = false;
            return;
        }

        if (!challengeHolder) {
            Debug.LogWarning("Please Assign challenge holder variable");
            return;
        }
        //Destroy Challenges First Then Create them randomly
        for (int i = 0; i < challengeHolder.transform.childCount; i++){
            Destroy(challengeHolder.transform.GetChild(i).gameObject);
        }
        int randomSelect = 0;
        GameObject instChallenge;
        for (int i = 0; i < challengeSpawnPoints.Length; i++){
            randomSelect = Random.Range(0, challengesArray.Length);
            if (challengesArray[randomSelect] != null)
            {
                instChallenge = Instantiate(challengesArray[randomSelect], challengeSpawnPoints[i].position, Quaternion.identity) as GameObject;
                instChallenge.transform.parent = challengeHolder.transform;
            }
        }
    }


}
