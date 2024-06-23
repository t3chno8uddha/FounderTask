using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    public string titleString = "Questions \n Game";
    public QuestionEvent[] questions; // Array of QuestionEvent objects
    public int pointTotal; // Total points accumulated
    int questionIndex; // Index of current question

    public TMP_Text questionText; // Text component to display the question
    public AnswerButton questionButtonPrefab; // Prefab for answer buttons

    List<GameObject> currentAnswers = new List<GameObject>(); // List to hold current answer buttons

    public Transform questionParent; // Parent transform for instantiated answer buttons

    public TMP_Text questionCounter; // Text component to display question index

    public float processingTime = 1.5f; // Time delay between questions
    bool processingQuestion = false; // Flag to prevent multiple answer checks

    public GameObject gameOverScreen; // Screen shown at the end of the game

    AudioSource aSource; // Audio source component
    public AudioClip correctSound, wrongSound; // Audio clips for correct and wrong answers
    public GameObject correctParticle; // Particle effect for correct answers

    void Start()
    {
        aSource = GetComponent<AudioSource>(); // Get AudioSource component
    }

    void UpdateCounter()
    {
        questionCounter.text = questionIndex + "/" + questions.Length; // Update UI with current question index
    }

    public void GetQuestion()
    {
        ClearAnswers(); // Clear previous answer buttons

        if (questionIndex < questions.Length) // If there are more questions
        {
            PrintQuestion(questions[questionIndex]); // Display the next question

            questionIndex++; // Move to the next question

            UpdateCounter(); // Update the question index counter
        }
        else // If all questions have been answered
        {
            GameOver(); // End the game
        }
    }

    void GameOver()
    {
        string finishText = "";

        if (pointTotal <= 1)
        {
            finishText = "Terrible..."; // Feedback for low score
        }
        else if (pointTotal == questions.Length)
        {
            finishText = "Great job!"; // Feedback for perfect score
        }
        else
        {
            finishText = "Good job."; // General positive feedback
        }

        questionCounter.text = ""; // Clear question counter

        gameOverScreen.SetActive(true); // Show game over screen
        questionText.text = pointTotal + "/" + questions.Length + "\n" + finishText; // Display final score and feedback
    }

    public void ResetGame()
    {
        questionIndex = 0;
        questionText.text = titleString;
    }

    void PrintQuestion(QuestionEvent question)
    {
        questionText.text = question.question; // Display the question text

        foreach(QuestionAnswer answer in question.answers) // Iterate through each answer in the question
        {
            GameObject answerObject = Instantiate(questionButtonPrefab.gameObject, questionParent.position, questionParent.rotation, questionParent); // Instantiate answer button
            AnswerButton answerButton = answerObject.GetComponent<AnswerButton>(); // Get AnswerButton component

            answerButton.button.onClick.AddListener(() => CheckAnswer(answerButton)); // Add listener for answer button
            answerButton.answerText.text = answer.answer; // Set answer text
            answerButton.answer = answer; // Assign answer data

            currentAnswers.Add(answerObject); // Add answer button to list
        }
    }

    void CheckAnswer(AnswerButton newAnswer)
    {
        if (!processingQuestion) // If not already processing an answer
        {
            AnswerButton correctAnswer = new AnswerButton(); // Variable to hold correct answer button

            foreach (GameObject answerObject in currentAnswers) // Iterate through current answer buttons
            {
                AnswerButton answer = answerObject.GetComponent<AnswerButton>(); // Get AnswerButton component

                if (answer.answer.correct) // If answer is correct
                {
                    correctAnswer = answer; // Assign it as correct answer
                }
            }

            correctAnswer.buttonRenderer.color = Color.green; // Highlight correct answer button
            
            if (correctAnswer != newAnswer) // If selected answer is incorrect
            {
                aSource.PlayOneShot(wrongSound); // Play wrong answer sound
                newAnswer.buttonRenderer.color = Color.red; // Highlight selected answer as wrong
            }
            else // If selected answer is correct
            {
                aSource.PlayOneShot(correctSound); // Play correct answer sound
                Instantiate(correctParticle, correctAnswer.transform.position, correctAnswer.transform.rotation); // Instantiate correct answer particle effect
                pointTotal++; // Increase point total
            }

            StartCoroutine(NextQuestionTimer()); // Start coroutine to move to next question
        }
    }

    IEnumerator NextQuestionTimer()
    {
        processingQuestion = true; // Set processing flag

        yield return new WaitForSeconds(processingTime); // Wait for processing time

        GetQuestion(); // Display next question
        processingQuestion = false; // Reset processing flag
    }

    void ClearAnswers()
    {
        foreach(GameObject answer in currentAnswers) // Iterate through current answer buttons
        {
            Destroy(answer); // Destroy each answer button
        }
        currentAnswers.Clear(); // Clear the list of current answer buttons
    }
}

[Serializable] public class QuestionAnswer
{
    public string answer; // Text of the answer
    public bool correct; // Whether the answer is correct or not
}