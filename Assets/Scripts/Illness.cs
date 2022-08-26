using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Illness : MonoBehaviour
{
    public GameObject player;
    public int sightLength = 8;
    public int enemySpeed;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Awake()
    {
        this.rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(this.gameObject.transform.position, player.transform.position) < sightLength){
            Vector3 dir = (player.transform.position - this.gameObject.transform.position).normalized;
            //rb.velocity = new Vector2(enemySpeed, dir);
        }
    }
}
