using UnityEngine;

[CreateAssetMenu(fileName = "Question", menuName = "Questions/New Question")]
public class QuestionEvent : ScriptableObject
{
    public string question; // The question of the question
    public QuestionAnswer[] answers; // The answers to the question
}