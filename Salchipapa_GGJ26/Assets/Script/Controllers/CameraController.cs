using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public float smoothspeed;

    private Vector3 targetPos, newPos;

    public Vector3 minPos, maxPos;

    private void LateUpdate()
    {
        if (transform.position != Player.position)
        {
            targetPos = Player.position;

            Vector3 camBoundaryPos = new Vector3
            (
                Mathf.Clamp(targetPos.x, minPos.x, maxPos.x),
                Mathf.Clamp(targetPos.y, minPos.y, maxPos.y),
                Mathf.Clamp(targetPos.z, minPos.z, maxPos.z)
            );

            newPos = Vector3.Lerp(transform.position, camBoundaryPos, smoothspeed * Time.deltaTime);
            transform.position = newPos;
        }
    }
}
