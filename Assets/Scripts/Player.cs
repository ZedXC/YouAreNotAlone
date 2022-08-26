using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float playerSpeed = 10f;
    public Rigidbody2D rb;
    private bool moving;
    public GameObject flashLight;
    private InputField input;
    public GameObject loseScreen;
    public Animator anim;
    private bool isDead = false;
    public int level = 0;
    public bool talking = false;

    // Start is called before the first frame update
    void Start()
    {   
        nextLevel();
    }

    // Update is called once per frame
    void Update()
    {
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
        if(talking){ return; }
         Vector2 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rb.velocity = new Vector2(playerSpeed * move.x, playerSpeed * move.y);
    }

    public void gameOver()
    {
        if (!isDead){
            anim.SetTrigger("Die");
            playerSpeed = 0f;
            Debug.Log("Game over");
            loseScreen.SetActive(true);
            rb.constraints =
                RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        }
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

    public void nextLevel(){
        MapMaker m = this.gameObject.AddComponent<MapMaker>();
        if(level == 0){
            m.makeMap(5, 5, 4, 3, this);
        }else if(level == 1){
             m.makeMap(10, 10, 15, 3, this);
        }else if(level == 2){
             m.makeMap(15, 15, 30, 4, this);
        }else if(level == 3){
            win();
        }
        level++;
    }

    private void win(){
        SceneManager.LoadScene("WinScene"); 
    }
}
