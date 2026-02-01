using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] float typingSpeed = 0.02f;

    private TMP_Text dialogueText;
    private GameObject nextPointer; // El punterito que indica que terminó el texto

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        // Buscamos en los hijos del objeto padre (nuestros hermanos)
        Transform parent = transform.parent;

        // Nota: Si tienes dos textos, esto puede fallar. Mejor usa esto:

        TMP_Text[] textos = parent.GetComponentsInChildren<TMP_Text>(true);
        foreach (var t in textos)
        {
            if (t.gameObject.name == "Text") dialogueText = t;
        }

        // Para el puntero, asegúrate que el nombre en nextPointer jerarquía sea EXACTO
        nextPointer = parent.Find("TextBox/NextPointer")?.gameObject;

        if (dialogueText == null) Debug.LogError("¡No encontré dialogueText!");
    }

    private void Start()
    {
        if (dialogueData != null && dialogueData.lineas.Length > 0)
        {
            ShowLine();
        }
    }

    private void Update()
    {
        // Usamos el nuevo Input System como en tu código anterior
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isTyping)
            {
                // Si está escribiendo, completamos el texto instantáneamente
                CompleteLine();
            }
            else
            {
                AdvanceDialogue();
            }
        }
    }

    private void ShowLine()
    {
        var line = dialogueData.lineas[currentLineIndex];   

        // Aplicamos la fuente que configuramos en el ScriptableObject
        dialogueText.font = line.fontAsset;

        if (nextPointer != null) nextPointer.SetActive(false);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeEffect(line.dialogueText));
    }

    IEnumerator TypeEffect(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in fullText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        EndLine();
    }

    private void CompleteLine()
    {
        StopCoroutine(typingCoroutine);
        dialogueText.text = dialogueData.lineas[currentLineIndex].dialogueText;
        EndLine();
    }

    private void EndLine()
    {
        isTyping = false;
        if (nextPointer != null) nextPointer.SetActive(true); // Aparece el punterito
    }

    public void AdvanceDialogue()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogueData.lineas.Length)
        {
            ShowLine();
        }
        else
        {
            // Final del diálogo
            transform.parent.gameObject.SetActive(false);
        }
    }
}
