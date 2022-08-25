using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{   
    public float playerSpeed = 500f;
    public Rigidbody2D rb;
    private bool moving;
    public float notMovingDrag = 2f;
    public float movingDrag = 100f;
    public GameObject flashLight;
    private InputField input;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if(move.x != 0 || move.y != 0){ moving = true; }
        rb.AddForce(move * Time.deltaTime * playerSpeed);
        rb.drag = moving?notMovingDrag:movingDrag;
        if(Input.GetKey(KeyCode.Z)){
            flashLight.transform.RotateAround(this.transform.position, Vector3.forward, 180*Time.deltaTime );
        }
        if(Input.GetKey(KeyCode.C)){
            flashLight.transform.RotateAround(this.transform.position, Vector3.forward, -180*Time.deltaTime);
        }
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
