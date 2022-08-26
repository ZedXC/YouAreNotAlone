using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class dialogue : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public GameObject player;
    public string[,] dialogueOptions = new string[6,5];
    private string[] sentences;
    private int index = 0;
    private int dialoguePath = 3;
    public float typingSpeed = 0.05f;
    public GameObject continueButton;
    public GameObject supportImageOne;
    public GameObject supportImageTwo;
    public GameObject supportImageThree;
    public GameObject blockImage;

    void Start(){
        //Angry
        dialogueOptions[0,0] = "WHAT DO YOU WANT?!";
        dialogueOptions[0,1] = "You ;)";
        //Lustful
        dialogueOptions[1,0] = "UUUN JAAA BABAB GOOOL";
        dialogueOptions[1,1] = "GABAGOOL";
        dialogueOptions[1,2] = "HABBA GUBBA WUBBA GOO";
        //Brainy
        dialogueOptions[2,0] = "Jingle bells jingle bells";
        dialogueOptions[2,1] = "Jingle bell rock";
        //BLANK
        dialogueOptions[3,0] = "what?";
    }

    void Update()
    {
        //checks for if the text display has the full sentence before turning on the continue button
        if (textDisplay.text == dialogueOptions[dialoguePath,index]) {
            continueButton.SetActive(true);
        }
    }

    public void OnActivation(int path)
    {
        blockImage.SetActive(true);
        if(path == 0){
            supportImageOne.SetActive(true);
        }
        else if(path == 1){
            supportImageTwo.SetActive(true);
        }
        else if(path == 2){
            supportImageThree.SetActive(true);
        }
        dialoguePath = path;
        StartCoroutine(Type());
    }

    IEnumerator Type() {
        //displays each individual letter one by one every given amount of time\
        foreach (char letter in dialogueOptions[dialoguePath,index].ToCharArray()) {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

    }

    public void NextSentence() {
        //a function that controls what happens when the sentences finish and how the continue button will work.
        continueButton.SetActive(false);

        //if there is more in the prewritten text go to the next part.
        if (index < dialogueOptions.GetLength(0) - 1 && dialogueOptions[dialoguePath,index+1] != null)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        //end talking with user and return to normal
        else {
            textDisplay.text = "";
            continueButton.SetActive(false);
            blockImage.SetActive(false);
            supportImageOne.SetActive(false);
            supportImageTwo.SetActive(false);
            supportImageThree.SetActive(false);
            index = 0;
        }
    }
}
