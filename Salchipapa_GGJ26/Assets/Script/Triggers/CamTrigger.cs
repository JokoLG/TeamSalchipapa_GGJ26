using UnityEngine;

public class CamTrigger : MonoBehaviour
{
    public Vector3 newCamPos, newPlayerPos;

    CameraController camController;

    private void Start()
    {
        camController = Camera.main.GetComponent<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            camController.minPos += newCamPos;
            camController.maxPos += newCamPos;

            other.transform.position += newPlayerPos;
        }

    }
}
