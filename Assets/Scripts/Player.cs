using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private float playerSpeedBase;
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
    public GameObject[] spoons = new GameObject[3];
    private int spoonsMax;
    public int spoonLength = 8;
    public AudioSource audio;
    public AudioSource click;
    private MapMaker m;

    // Start is called before the first frame update
    void Start()
    {   
        playerSpeedBase = playerSpeed;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            useSpoon();
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
                RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void resetGame()
    {
        playerSpeed = playerSpeedBase;
        rb.constraints =
                RigidbodyConstraints2D.FreezeRotation;
        anim.SetTrigger("ResetGame");
        loseScreen.SetActive(false);
        level = level-1;
        m.destroyMap();
        nextLevel();
    }

    void OnCollisionEnter2D(Collision2D gameObjectInformation)
    {
        if (gameObjectInformation.gameObject.tag == "Illness")
        {
            gameObjectInformation.gameObject.GetComponent<Illness>().onPlayerDeath();
            gameOver();
        }
    }

    public Vector2 degreeToVector(float degree)
    {
        return (Vector2)(Quaternion.Euler(0, 0, degree) * Vector2.right);
    }

    public void nextLevel(){
        this.m = this.gameObject.AddComponent<MapMaker>();
        if(level == 0){
            activateSpoons(level);
            m.makeMap(4, 4, 4, 3, this);
        }else if(level == 1){
            activateSpoons(level);
            m.makeMap(5, 5, 5, 3, this);
        }else if(level == 2){
            activateSpoons(level);
            m.makeMap(6, 6, 7, 4, this);
        }else if(level == 3){
            win();
        }
        level++;
    }

    private void win(){
        SceneManager.LoadScene("WinScene"); 
    }

    private void activateSpoons(int lvl){
        if(lvl == 0){
            spoonsMax = 1;
            spoons[0].SetActive(true);
            spoons[1].SetActive(false);
            spoons[2].SetActive(false);
        }
        if(lvl == 1){
            spoonsMax = 2;
            spoons[0].SetActive(true);
            spoons[1].SetActive(true);
            spoons[2].SetActive(false);
        }
        if(lvl == 2){
            spoonsMax = 3;
            spoons[0].SetActive(true);
            spoons[1].SetActive(true);
            spoons[2].SetActive(true);
        }
    }

    private void useSpoon(){
        if(spoonsMax > 0){
            anim.SetTrigger("Burst");
            audio.Play();
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Illness");
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (
                        Vector3.Distance(
                            this.gameObject.transform.position,
                            enemies[i].GetComponent<Illness>().getPosition()
                        ) < spoonLength
                    )
                    {
                        Destroy(enemies[i]);
                    }
            }
            spoonsMax--;
            if(spoons[2].activeSelf == true){
                spoons[2].SetActive(false);
            }
            else if(spoons[1].activeSelf == true){
                spoons[1].SetActive(false);
            }
            else if(spoons[0].activeSelf == true){
                spoons[0].SetActive(false);
            }
        }
    }

    public void findTorch(){
        click.Play();
        flashLight.SetActive(true);
    }
}
