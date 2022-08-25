using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public GameObject dialogueManager;
    public int path;
    private bool interactable = true;
    private void OnTriggerEnter2D(Collider2D gameObjectInformation)
    {
        if (gameObjectInformation.tag == "Player")
        {
            if(interactable){
                Debug.Log("Detected");
                dialogueManager.GetComponent<dialogue>().OnActivation(path);
                interactable = false;
            }
        }
    }
}
