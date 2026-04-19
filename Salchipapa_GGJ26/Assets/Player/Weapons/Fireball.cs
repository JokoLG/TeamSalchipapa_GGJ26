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

    public void Init(Transform directionSource)
    {
        dir = directionSource.right;
        rb.linearVelocity = dir * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Brick brick = other.GetComponent<Brick>();
        if (brick != null)
        {
            Destroy(gameObject);
            return;
        }

        Spotlight spotlight = other.GetComponent<Spotlight>();
        if (spotlight != null)
        {
            spotlight.isActive = true;
            Destroy(gameObject);
            return;
        }

        EneCom ene = other.GetComponent<EneCom>();
        if (ene != null)
        {
            if (ene.IsStunnable)
                ene.Stun();
            else
                ene.HitFireball(fireballKnockback, dir);

            Destroy(gameObject);
            return;
        }
    }
}