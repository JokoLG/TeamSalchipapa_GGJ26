using UnityEngine;

public class CamTrigger : MonoBehaviour
{
    [Header("Nuevos Límites para esta Sala")]
    public Vector2 newMinPos;
    public Vector2 newMaxPos;

    [Header("Teletransporte del Jugador")]
    public Vector3 playerTeleportOffset;

    CameraController camController;

    private void Start()
    {
        // Buscamos el componente en la cámara principal
        camController = Camera.main.GetComponent<CameraController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Trigger detectado con el Jugador!"); // Mira esto en la consola

            if (camController != null)
            {
                camController.minPos = newMinPos;
                camController.maxPos = newMaxPos;
                other.transform.position += playerTeleportOffset;
            }
            else
            {
                Debug.LogError("No se encontró el CameraController. ¿Está el script en la cámara?");
            }
        }
    }


}