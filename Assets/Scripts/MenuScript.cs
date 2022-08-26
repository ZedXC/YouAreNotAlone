using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public string sceneLoadingString;

    public void quit()
    {
        Application.Quit();
        Debug.Log("QUITTING");
    }

    public void reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadSceneString()
    {
        SceneManager.LoadScene(sceneLoadingString);
    }
}
