using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLost : MonoBehaviour
{
    public GameObject loseScreen;

    void Lose()
    {
        loseScreen.SetActive(true);
    }
}
