using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   
    public float moveSpeed = 0.1f;

    //public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        //animator.SetFloat("Speed", moveX * moveSpeed);
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        Vector2 newPos = new Vector2(gameObject.transform.position.x + moveDirection.x * moveSpeed * Time.deltaTime, gameObject.transform.position.y + moveDirection.y * moveSpeed * Time.deltaTime);
        this.gameObject.transform.position = newPos;
    }


    //TODO
    public void gameOver()
    {
        Debug.Log("Game over");
        Destroy(this.gameObject);
    }

    public Vector2 degreeToVector(float degree)
    {
        return (Vector2)(Quaternion.Euler(0, 0, degree) * Vector2.right);
    }
}
