using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    public Button button; // Answer button component
    public Image buttonRenderer; // Answer renderer
    public TMP_Text answerText; // Answer text
    public QuestionAnswer answer; // The actual answer
}