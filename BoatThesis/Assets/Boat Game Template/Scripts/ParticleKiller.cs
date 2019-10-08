using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Destroys the particle system after it stops playing
/// </summary>
public class ParticleKiller : MonoBehaviour {
    ParticleSystem ps;
	// Use this for initialization
	void Start () {
        if (transform.GetComponent<ParticleSystem>())
            ps = transform.GetComponent<ParticleSystem>();
        if (ps)
            StartCoroutine(Kill());
	}
    //Killing routine
    IEnumerator Kill() {
        while (ps) {
            if (!ps.IsAlive())
                Destroy(gameObject);
            yield return null;
        }
        yield return null;
    }

    
}
