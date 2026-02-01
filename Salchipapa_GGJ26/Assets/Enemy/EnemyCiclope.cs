using UnityEngine;

public class EnemyCiclope : MonoBehaviour
{
    public Transform player;

    [Header("Movimiento")]
    public float rangoPersecusion = 5f;
    public float velocidadMovimiento = 2f;

    private Rigidbody2D rb;
    private Vector2 direccion;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= rangoPersecusion)
        {
            Vector2 diff = player.position - transform.position;

            
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
                direccion = new Vector2(Mathf.Sign(diff.x), 0);
            else
                direccion = new Vector2(0, Mathf.Sign(diff.y));
        }
        else
        {
            direccion = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + direccion * velocidadMovimiento * Time.fixedDeltaTime);
    }
}

