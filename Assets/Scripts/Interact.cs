using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public GameObject dialogueManager;
    public int path;
    public bool interactable = true;
    public float sightLength = 8;
    public Animator anim;

    private void OnTriggerEnter2D(Collider2D gameObjectInformation)
    {
        if (gameObjectInformation.tag == "Player")
        {
            if (interactable)
            {
                Debug.Log("Detected");
                anim.SetTrigger("Interacted");
                Player player = GameObject.Find("Player").GetComponent<Player>();
                player.talking = true;
                player.rb.velocity = new Vector2(0,0);
            
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Illness");
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (
                        Vector3.Distance(
                            this.gameObject.transform.position,
                            enemies[i].GetComponent<Illness>().getPosition()
                        ) < sightLength
                    )
                    {
                        Destroy(enemies[i]);
                    }
                }
                dialogueManager.GetComponent<dialogue>().OnActivation(path);
                interactable = false;
            }
        }
    }
}
