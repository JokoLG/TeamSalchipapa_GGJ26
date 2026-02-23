using UnityEngine;

public class BasicMov : MonoBehaviour
{
    [Header("Trigger")]
    public string targetTag = "Level";
    public Vector3 lastHitCenter;

    [Header("Object To Move")]
    public Transform objectToMove;

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        lastHitCenter = other.bounds.center;

        if (objectToMove != null)
        {
            Vector3 newPos = objectToMove.position;
            newPos.x = other.transform.position.x;
            newPos.y = other.transform.position.y;
            objectToMove.position = newPos;
        }
    }
}