using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public static Furniture Get { get; private set; }
    private void Awake(){ Get = this; }
    public GameObject Cabinet;
    public GameObject Chair;
    public GameObject Couch;
    public GameObject Crate;
    public GameObject DinnerTable;
    public GameObject DogBed;
    public GameObject Oven;
    public GameObject Plant;
    public GameObject Rug;
    public GameObject Table;
    public GameObject Wardrobe;
}
