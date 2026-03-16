using System.Collections;
using UnityEngine;
using TMPro;

public class DialogEvent : MonoBehaviour
{

    [Header("UI")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;

    [Header("Jugador")]
    public MonoBehaviour playerMovementScript;

    [Header("Mensaje")]
    [TextArea(3, 6)]
    public string message;

    public float typingSpeed = 0.05f;

    private bool isTyping = false;
    private bool finished = false;

    public void StartDialogue()
    {
        playerMovementScript.enabled = false;
        dialogueBox.SetActive(true);
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
        if (!finished) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueText.text = "";
        dialogueBox.SetActive(false);
        playerMovementScript.enabled = true;
        finished = false;
        this.enabled = false;
    }
}
