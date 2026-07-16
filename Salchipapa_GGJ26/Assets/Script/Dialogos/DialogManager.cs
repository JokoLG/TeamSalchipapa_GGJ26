using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour playersound;
    public Image backgroundImage; 
    public float typingSpeed = 0.05f;

    private bool isTyping = false;
    private bool finished = false;
    private string currentMessage;

    public void StartDialogue(string message, Color bgColor, Color txtColor)
    {
        currentMessage = message;

        backgroundImage.color = bgColor;
        dialogueText.color = txtColor;

        playerMovementScript.GetComponent<P_Movement>().isMoving = false;
        playersound.GetComponent<P_SoundHandler>().StopLoop();
        playerMovementScript.enabled = false;
        dialogueBox.SetActive(true);

        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        finished = false;
        dialogueText.text = "";

        foreach (char letter in currentMessage)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        finished = true;
    }

    void Update()
    {
        if (!dialogueBox.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = currentMessage;
                isTyping = false;
                finished = true;
            }
            else if (finished)
            {
                EndDialogue();
            }
        }
    }

    void EndDialogue()
    {
        dialogueText.text = "";
        dialogueBox.SetActive(false);
        playerMovementScript.enabled = true;
    }
}