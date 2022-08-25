using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public static Furniture Instance { get; private set; }
    private void Awake(){ Instance = this; }

}
