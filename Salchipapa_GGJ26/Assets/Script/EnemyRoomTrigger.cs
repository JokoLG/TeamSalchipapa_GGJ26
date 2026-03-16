using UnityEngine;

public class EnemyRoomTrigger : MonoBehaviour
{
    public EnemyMovement_Free enemy;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.ActivateEnemy();
        }
    }
}