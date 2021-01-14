using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayScript : MonoBehaviour
{
    public ShapeScript shapeScript; // Reference to shape script
    
    public GameObject demoOptions; // Options for demo
    public GameObject gameOptions; // Options for playing game again
    public GameObject gameTexts;  // Text used in game

    int timer = 0;   // Timer value
    public int score = 0;  // Current score
    int highScore = 0;  // High score

    public TextMeshProUGUI timerDisplay;    // Displays the timer
    public TextMeshProUGUI scoreDisplay;    // Displays your current score
    public TextMeshProUGUI highScoreDisplay;    // Displays your best score

    public GameObject penguin;  // Object used for hide-n-seek

    public bool timerActive = false;    // Is the timer active?
    public bool gameStart = false;  // Has the game started

    GameObject tempPenguin; // Temporary gameobject used to store the penguin

    public AudioClip countDownSound; // Sound used for countdown

    void Update()
    {
        if (timerActive)
        {
            scoreDisplay.text = "Score: " + score.ToString();
            if (score >= highScore)
            {
                highScore = score;
                highScoreDisplay.text = "Best: " + highScore.ToString();
            }
        }
    }

    public void PlayGame ()
    {
        gameStart = true;
        // Stop potential dancing and reset the shape's position
        shapeScript.StopDance();
        shapeScript.shapeDance = false;
        shapeScript.transform.position = Vector3.zero;

        // Set the propper displays
        demoOptions.SetActive(false);
        gameOptions.SetActive(false);
        gameTexts.SetActive(true);

        StartCoroutine(BeginTimer());
    }

    IEnumerator BeginTimer ()
    {
        tempPenguin = GameObject.Instantiate(penguin, new Vector3(-1000f, -1000f, -1000f), Quaternion.identity);
        score = 0;
        timer = 3;
        while (timer > 0)
        {
            timerDisplay.text = timer.ToString();
            GetComponent<AudioSource>().PlayOneShot(countDownSound);
            yield return new WaitForSeconds(1f);
            timer--;
        }

        tempPenguin.GetComponent<PenguinColorScript>().Hide();

        timerDisplay.text = "GO";
        GetComponent<AudioSource>().PlayOneShot(countDownSound);
        yield return new WaitForSeconds(1f);
        timerActive = true;
        timer = 30;


        while(timer > 0)
        {
            timerDisplay.text = timer.ToString();
            if(timer <= 5)
            {
                GetComponent<AudioSource>().PlayOneShot(countDownSound);
            }
            timer--;
            yield return new WaitForSeconds(1f);
        }

        timerActive = false;
        GameObject.Destroy(tempPenguin);
        GetComponent<AudioSource>().PlayOneShot(countDownSound);
        yield return new WaitForSeconds(0.1f);
        GetComponent<AudioSource>().PlayOneShot(countDownSound);
        timerDisplay.text = "0";
        gameOptions.SetActive(true);

        yield return null;
    }

    public void GoBack ()
    {
        gameStart = false;
        demoOptions.SetActive(true);
        gameOptions.SetActive(false);
        gameTexts.SetActive(false);
    }
}
