using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightPickup : MonoBehaviour
{
    public GameObject player;
    public GameObject DestroyThis;

    private void OnTriggerEnter2D(Collider2D gameObjectInformation)
    {
        if (gameObjectInformation.tag == "Player")
        {
            player.GetComponent<Player>().findTorch();
            Destroy(DestroyThis);
        }
    }
}
