using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public SimpleObjectPool answersButtonObjectPool;
    public Text questionDisplayText;
    public Transform answerButtonParent;

    public Text scoreDisplayText;
    public Text timeRemainingDisplayText;
    public GameObject questionDisplayPanel;
    public GameObject roundOverDisplayPanel;

    private AudioSource audioSource;
    
    public AudioClip startSound;
    public AudioClip correctAnswerSound;
    public AudioClip wrongAnswerSound;
    public AudioClip endRoundSound;

    private DataController dataController;
    private RoundData currentRoundData; 
    private QuestionData[] questionPool;

    private bool isRoundActive; 
    private float timeRemaining;
    private int questionIndex;
    private int playerScore;

    private List <GameObject> answerButtonGameObjects = new List<GameObject>();


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        dataController = FindObjectOfType<DataController> ();
        currentRoundData = dataController.GetCurrentRoundData ();
        questionPool = currentRoundData.questions;
        timeRemaining = currentRoundData.timeLimitInSeconds;
        UpdateTimeRemainingDisplay();

   
        playerScore = 0;
        questionIndex = 0;
        isRoundActive = true;

        ShowQuestion();

        audioSource.PlayOneShot(startSound);
    }


    public void AnswerButtonClicked(bool isCorrect)
    {
        if (isCorrect)
        {
            playerScore += currentRoundData.pointsAddedForCorrectAnswers;
            scoreDisplayText.text = "Score: " + playerScore.ToString();

            audioSource.PlayOneShot(correctAnswerSound);

        }

        else 
        {
            playerScore -= 5;
            scoreDisplayText.text = "Score: " + playerScore.ToString();

            audioSource.PlayOneShot(wrongAnswerSound);
        } 
        

        if (questionPool.Length > questionIndex + 1)
        {
            questionIndex++;
            ShowQuestion();
        }
        else 
        {
            EndRound();
        }
    }

    public void EndRound ()
    {
        isRoundActive = false;

        questionDisplayPanel.SetActive (false);
        roundOverDisplayPanel.SetActive (true);

        audioSource.PlayOneShot(endRoundSound);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScreen");
    }

    private void ShowQuestion()
    {
        RemoveAnswerButtons();
        QuestionData questionData = questionPool [questionIndex];
        questionDisplayText.text = questionData.questionText;
        
        for (int i = 0; i < questionData.answers.Length; i++)
        {
            GameObject answerButtonGameObject = answersButtonObjectPool.GetObject();
            answerButtonGameObject.transform.SetParent (answerButtonParent);
            answerButtonGameObjects.Add (answerButtonGameObject);
            
            AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
            answerButton.SetUp (questionData.answers [i]);
        }
         
    }
    
    private void RemoveAnswerButtons ()
    {
        while (answerButtonGameObjects.Count > 0)
        {
            answersButtonObjectPool.ReturnObject (answerButtonGameObjects [0]);
            answerButtonGameObjects.RemoveAt (0);
        }
    }

    private void UpdateTimeRemainingDisplay()
    {
        timeRemainingDisplayText.text = "Time: " + Mathf.Round(timeRemaining).ToString();
    }


    void Update()
    {
        if (isRoundActive)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeRemainingDisplay();
            if (timeRemaining <= 0)
            {
                EndRound();
            }
        }
    }
}
