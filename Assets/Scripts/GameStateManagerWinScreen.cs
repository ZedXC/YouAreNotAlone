using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManagerWinScreen : MonoBehaviour
{
    public GameObject pauseScreen;
    public bool paused;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            Debug.Log("pausing");
            pause();
        }
    }
    public void pause(){
        if(!paused){
            paused = true;
            pauseScreen.SetActive(true);
        }
        else{
            paused = false;
            pauseScreen.SetActive(false);
        }
    }
    public void quit(){
        Application.Quit();
        Debug.Log("QUITTING");
    }
    public void reload(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadSceneNewGameString(){
        SceneManager.LoadScene("SampleScene"); 
    }
    public void LoadSceneBackToMenuString(){
        SceneManager.LoadScene("TitleScreen"); 
    }
}
