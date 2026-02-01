using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 1.5f;

    [Header("Combat")]
    [SerializeField] private float fireballKnockback = 1f;

    private Rigidbody2D rb;

    private Vector2 dir;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Pass the transform that defines direction (weapon, pivot, player, etc.)
    public void Init(Transform directionSource)
    {
        dir = directionSource.right; // ← uses Z rotation automatically
        rb.linearVelocity = dir * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyMovement_Free enemy = other.GetComponent<EnemyMovement_Free>();
        if (enemy == null) return;

        enemy.HitFireball(fireballKnockback, dir);
        Destroy(gameObject);
    }
}
