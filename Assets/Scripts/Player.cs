using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float playerSpeed = 10f;
    public Rigidbody2D rb;
    private bool moving;
    public GameObject flashLight;
    private InputField input;
    public GameObject loseScreen;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        MapMaker m = this.gameObject.AddComponent<MapMaker>();
        m.makeMap(5, 5, 20, this);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rb.velocity = new Vector2(playerSpeed * move.x, playerSpeed * move.y);
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.J))
        {
            flashLight.transform.RotateAround(
                this.transform.position,
                Vector3.forward,
                180 * Time.deltaTime
            );
        }
        if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.L))
        {
            flashLight.transform.RotateAround(
                this.transform.position,
                Vector3.forward,
                -180 * Time.deltaTime
            );
        }
    }

    public void gameOver()
    {
        anim.SetTrigger("Die");
        playerSpeed = 0f;
        Debug.Log("Game over");
        loseScreen.SetActive(true);
    }

    void OnCollisionEnter2D(Collision2D gameObjectInformation)
    {
        if (gameObjectInformation.gameObject.tag == "Illness")
        {
            gameOver();
        }
    }

    public Vector2 degreeToVector(float degree)
    {
        return (Vector2)(Quaternion.Euler(0, 0, degree) * Vector2.right);
    }
}
