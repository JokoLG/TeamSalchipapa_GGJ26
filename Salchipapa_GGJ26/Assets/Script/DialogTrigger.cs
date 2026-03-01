using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [TextArea(3, 6)]
    public string message;

    public DialogManager dialogueManager;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        hasTriggered = true;
        dialogueManager.StartDialogue(message);
    }
}


