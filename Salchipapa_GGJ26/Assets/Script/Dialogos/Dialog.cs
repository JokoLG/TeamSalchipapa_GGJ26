using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialog : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;

    [Header("Mensaje")]
    [TextArea(3, 6)]
    public string message;

    public float typingSpeed = 0.05f;

    private bool isTyping = false;
    private bool finished = false;

    void Start()
    {
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        finished = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = message;
                isTyping = false;
                finished = true;
            }
            else if (finished)
            {
                SceneManager.LoadScene("GameScene");
            }
        }
    }
}

