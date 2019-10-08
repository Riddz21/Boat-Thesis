using UnityEngine;
using System.Collections;

public class PowerUps : MonoBehaviour {
    //burst effect
    public GameObject burstEffects;
    //destroy after the touch
    public bool destroyMe = true;
    //get the target to move towards it
    Transform target;

    //power up types
    public enum powerUpTypes {
        Shield, Gasoline, Coin, Magnet,Gem
    };

    public powerUpTypes powerUpType;

    //Call functions based on power up type
    public void InitializePowerUp(Transform other) {
        switch (powerUpType) {
            case powerUpTypes.Shield: other.SendMessage("EnableShield", SendMessageOptions.RequireReceiver); break;
            case powerUpTypes.Gasoline: other.SendMessage("FillUpGas", SendMessageOptions.RequireReceiver); break;
            case powerUpTypes.Coin: other.SendMessage("AddCoin", SendMessageOptions.RequireReceiver); break;
            case powerUpTypes.Magnet: other.SendMessage("EnableMagnet", SendMessageOptions.RequireReceiver); break;
            case powerUpTypes.Gem: other.SendMessage("AddGem", SendMessageOptions.RequireReceiver); break;
        }
        if (burstEffects) {
            GameObject instBurst = Instantiate(burstEffects, transform.position, Quaternion.identity) as GameObject;
            instBurst.transform.parent = transform.parent;
        }
        if (destroyMe)
            Destroy(gameObject);
    }

    IEnumerator MoveCoin()
    {
        while (true) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, transform.position.y, target.position.z), Time.deltaTime * 10.0f);
            yield return null;
        }
    }

    public void SetCoinMagnet(Transform other) {
        if (target || powerUpType !=  powerUpTypes.Coin) {
            return;
        }
        target = other;
        StartCoroutine(MoveCoin());
    }
}
