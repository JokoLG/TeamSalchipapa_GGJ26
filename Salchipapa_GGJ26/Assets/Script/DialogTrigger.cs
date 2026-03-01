using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [Header("Referencias")]
    public DialogEvent dialogEvent;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.GetComponent<MonoBehaviour>() != null) // Detecta cualquier objeto que tenga scripts
        {
            hasTriggered = true;
            dialogEvent.StartDialogue();
        }
    }
}

