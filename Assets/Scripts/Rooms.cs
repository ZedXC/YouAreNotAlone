using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rooms : MonoBehaviour
{
    public static Rooms Instance { get; private set; }
    private void Awake(){ Instance = this; }
    public GameObject Tee;
    public GameObject corridor;
    public GameObject rightTurn;
    public GameObject startEnd;
    public GameObject cross;
    public GameObject deadEnd;
}
