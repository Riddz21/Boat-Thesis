using UnityEngine;
using System.Collections;

public class RotateStuff : MonoBehaviour {
    public Vector3 rotationVector;
	// Use this for initialization
	void OnEnable () {
        StartCoroutine(RotateMe());
	}
    //Roate stuff as long as they are alive
    IEnumerator RotateMe() {

        while (true) {

            transform.Rotate(rotationVector);

            yield return null;

        }
    }
}
