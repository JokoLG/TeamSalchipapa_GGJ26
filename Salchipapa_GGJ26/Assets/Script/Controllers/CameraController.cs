using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public float smoothspeed = 0.125f; // Ajusta este valor (ej: 0.125)

    [Header("Límites de la Cámara")]
    public Vector2 minPos; // Usamos Vector2 para ignorar el eje Z
    public Vector2 maxPos;

    private void LateUpdate()
    {
        if (Player == null) return;

        // Calculamos la posición deseada en X e Y con Clamp
        float targetX = Mathf.Clamp(Player.position.x, minPos.x, maxPos.x);
        float targetY = Mathf.Clamp(Player.position.y, minPos.y, maxPos.y);

        // Creamos el Vector3 manteniendo el Z original de la cámara
        Vector3 targetPos = new Vector3(targetX, targetY, transform.position.z);

        // Movimiento suavizado
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothspeed);
    }
}