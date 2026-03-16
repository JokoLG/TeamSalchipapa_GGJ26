using System.Collections;
using UnityEngine;
using TMPro;

public class DialogGame : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;
    public MonoBehaviour playerMovementScript; // arrastra aquí tu script de movimiento

    [Header("Mensaje")]
    [TextArea(3, 6)]
    public string message;

    public float typingSpeed = 0.05f;

    private bool isTyping = false;
    private bool finished = false;

    void Start()
    {
        // Bloquear movimiento
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
        if (finished && Input.GetKeyDown(KeyCode.Space))
        {
            EndDialogue();
        }

        if (isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            dialogueText.text = message;
            isTyping = false;
            finished = true;
        }
    }

    void EndDialogue()
    {
        dialogueText.text = "";            // Borra el texto
        dialogueBox.SetActive(false);      // Oculta la caja
        playerMovementScript.enabled = true; // Activa movimiento
        this.enabled = false;              // Desactiva este script
    }
}
