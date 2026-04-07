using UnityEngine;

public class BasicMov : MonoBehaviour
{
    [Header("Trigger")]
    public string targetTag = "Level";
    public string targetTag2 = "FinalLevel";
    public Vector3 lastHitCenter;

    [Header("Object To Move")]
    public Transform objectToMove;
    public Camera cam;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(targetTag) || other.CompareTag(targetTag2))
        {
            cam.orthographicSize = 10.8f;

            lastHitCenter = other.bounds.center;

            if (objectToMove != null)
            {
                Vector3 newPos = objectToMove.position;
                newPos.x = other.transform.position.x;
                newPos.y = other.transform.position.y;
                objectToMove.position = newPos;
            }

            if (other.CompareTag(targetTag2))
                cam.orthographicSize = 10.8f + 5.4f;
        }
    }
}