using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class dialogue : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public GameObject player;
    public string[,] dialogueOptions = new string[6, 5];
    private string[] sentences;
    private int index = 0;
    private int dialoguePath = 3;
    public float typingSpeed = 0.05f;
    public GameObject continueButton;
    public GameObject supportImageOne;
    public GameObject supportImageTwo;
    public GameObject supportImageThree;
    public GameObject supportImageFour;
    public GameObject blockImage;
    public AudioSource talkingSound;

    void Start()
    {
        //Mother
        dialogueOptions[0, 0] = "Mother: Its okay. Ill get rid of the bad thoughts for now";
        dialogueOptions[0, 1] = "Mother: stay safe on your journey";
        dialogueOptions[0, 2] = "Mother: remember I love you";
        //Friend
        dialogueOptions[1, 0] = "Friend: Hey man I understand this is tough";
        dialogueOptions[1, 1] = "Friend: Dont worry I'll get rid of the bad thoughts for now";
        dialogueOptions[1, 2] = "Friend: stay safe okay? you got this!";
        //Therapist
        dialogueOptions[2, 0] = "Therapist: If you need anything just let me know";
        dialogueOptions[2, 1] = "Therapist: I am here to help you with whatever you need";
        dialogueOptions[2, 2] = "Therapist: You're doing so well just a little bit longer";
        //Homuncuhunk
        dialogueOptions[3, 0] = "Homuncuhunk: OH YEAH BABY";
        dialogueOptions[3, 1] = "Homuncuhunk: JUST A LITTLE BIT FURTHER YOU CAN DO IT";
        dialogueOptions[3, 2] = "Homuncuhunk: YOU JUST GOTTA BELIEVE!!!";
        //BLANK
        dialogueOptions[4, 0] = "what?";
        dialogueOptions[4, 0] = "How did you get this?";
        dialogueOptions[4, 0] = "Like seriously this shouldn't be possible. shits broken.";
    }

    void Update()
    {
        //checks for if the text display has the full sentence before turning on the continue button
        if (dialoguePath != -1 && textDisplay.text == dialogueOptions[dialoguePath, index])
        {
            continueButton.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Return))
            {
                NextSentence();
            }
        }
    }

    public void OnActivation(int path)
    {
        blockImage.SetActive(true);
        if (path == 0)
        {
            supportImageOne.SetActive(true);
        }
        else if (path == 1)
        {
            supportImageTwo.SetActive(true);
        }
        else if (path == 2)
        {
            supportImageThree.SetActive(true);
        }
        else if (path == 3)
        {
            supportImageFour.SetActive(true);
        }
        dialoguePath = path;
        StartCoroutine(Type());
    }

    IEnumerator Type()
    {
        //displays each individual letter one by one every given amount of time\
        foreach (char letter in dialogueOptions[dialoguePath, index].ToCharArray())
        {
            if(letter != ' '){
                talkingSound.Play();
            }
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextSentence()
    {
        //a function that controls what happens when the sentences finish and how the continue button will work.
        continueButton.SetActive(false);

        //if there is more in the prewritten text go to the next part.
        if (
            index < dialogueOptions.GetLength(0) - 1&& dialogueOptions[dialoguePath, index + 1] != null
        )
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        //end talking with user and return to normal
        else
        {
            textDisplay.text = "";
            continueButton.SetActive(false);
            blockImage.SetActive(false);
            supportImageOne.SetActive(false);
            supportImageTwo.SetActive(false);
            supportImageThree.SetActive(false);
            supportImageFour.SetActive(false);
            index = 0;
            dialoguePath = -1;
            GameObject.Find("Player").GetComponent<Player>().talking = false;
        }
    }

    public bool notTalking(){
        return dialoguePath == -1;
    }
}
