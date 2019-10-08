using UnityEngine;
using System.Collections;

public class Crocodile : MonoBehaviour {
    //croc mesh
    public Transform crocodileGraphic;
    //Croc Collider
    public Collider crocodileCollider;
    //movement speed
    public float speed = 5.0f;
    //reaching area
    public float reachArea = 5.0f;
    //travel area
    public float travelArea = 5.0f;

    int currentPoint = 1; //1 is right 0 is left
    
    //set all the variables
    void Start() {

        Vector3 rightPoint = new Vector3(transform.position.x + travelArea, transform.position.y, transform.position.z);
        Vector3 leftPoint = new Vector3(transform.position.x - travelArea, transform.position.y, transform.position.z);
        Vector3 newPos = Vector3.Lerp(rightPoint, leftPoint, Random.Range(0.0f, 1.0f));
        if (crocodileGraphic)
            crocodileGraphic.position = newPos;
        if (crocodileCollider)
            crocodileCollider.transform.position = newPos;
        

        StartCoroutine(Move());
    }
    //move left right
    IEnumerator Move() {
        if (!crocodileGraphic && crocodileCollider)
            yield return null;
        Vector3 rightPoint;
        Vector3 leftPoint;

        while (true) {
            rightPoint = new Vector3(transform.position.x + travelArea, transform.position.y, transform.position.z);
            leftPoint = new Vector3(transform.position.x - travelArea, transform.position.y, transform.position.z);

            switch (currentPoint) {
                case 0:
                    crocodileGraphic.position = Vector3.MoveTowards(crocodileGraphic.position, leftPoint,Time.deltaTime*speed);
                    crocodileCollider.transform.position = crocodileGraphic.position;
                    if (Vector3.Distance(crocodileGraphic.position, leftPoint) <= reachArea) {
                        currentPoint = 1;
                    }
                    crocodileGraphic.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    break;

                case 1:
                    crocodileGraphic.position = Vector3.MoveTowards(crocodileGraphic.position, rightPoint, Time.deltaTime * speed);
                    crocodileCollider.transform.position = crocodileGraphic.position;
                    if (Vector3.Distance(crocodileGraphic.position, rightPoint) <= reachArea)
                    {
                        currentPoint = 0;
                    }
                    crocodileGraphic.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
            }

            yield return null;

        }

    }
    // display path
    void OnDrawGizmos() {
        Vector3 rightPoint = new Vector3( transform.position.x + travelArea, transform.position.y, transform.position.z);
        Vector3 leftPoint = new Vector3(transform.position.x - travelArea, transform.position.y, transform.position.z);
        Gizmos.DrawWireSphere(rightPoint, reachArea);
        Gizmos.DrawWireSphere(leftPoint, reachArea);
        Gizmos.DrawLine(rightPoint, leftPoint);
    }
}
