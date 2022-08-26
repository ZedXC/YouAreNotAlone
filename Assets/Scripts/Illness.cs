using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Illness : MonoBehaviour
{
    public Transform player;
    public float sightLength = 8;
    public float enemySpeed = 3f;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        this.rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(this.gameObject.transform.position, player.position) < sightLength)
        {
            Vector3 dir = (player.position - this.gameObject.transform.position).normalized;
            rb.velocity = new Vector2(enemySpeed * dir.x, enemySpeed * dir.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D gameObjectInformation)
    {
        if (gameObjectInformation.tag == "FlashLight")
        {
            enemySpeed = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D gameObjectInformation)
    {
        if (gameObjectInformation.tag == "FlashLight")
        {
            enemySpeed = 3f;
        }
    }

    public Vector3 getPosition()
    {
        return this.gameObject.transform.position;
    }
}
