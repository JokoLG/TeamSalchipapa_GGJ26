using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BossSwordHB : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D hitboxCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private P_SoundHandler soundPlayer;

    [Header("Timing")]
    [SerializeField] private float startupTime = 0.28f;
    [SerializeField] private float activeTime = 0.12f;
    [SerializeField] private float recoveryTime = 0.20f;

    [Header("Combat")]
    [SerializeField] private float knockback = 1.2f;

    [Header("Colors")]
    [SerializeField] private Color hiddenColor = new Color(1f, 1f, 1f, 0f);
    [SerializeField] private Color warningColor = new Color(1f, 0f, 1f, 0.65f);
    [SerializeField] private Color activeColor = Color.white;

    private FacingDirection facing = FacingDirection.Right;
    private bool damageActive = false;
    private P_Health currentTarget;

    public bool HasTarget
    {
        get { return currentTarget != null && !currentTarget.IsDead; }
    }

    public P_Health CurrentTarget
    {
        get
        {
            if (HasTarget) return currentTarget;
            return null;
        }
    }

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (hitboxCollider == null)
            hitboxCollider = GetComponent<Collider2D>();

        hitboxCollider.enabled = true;
        spriteRenderer.color = hiddenColor;
    }

    public void SetFacing(FacingDirection dir)
    {
        facing = dir;
    }

    public IEnumerator PlayAttack()
    {
        damageActive = false;

        if (soundPlayer != null)
            soundPlayer.Play("OdySlash");

        spriteRenderer.color = warningColor;
        yield return new WaitForSeconds(startupTime);

        damageActive = true;
        spriteRenderer.color = activeColor;

        yield return new WaitForSeconds(activeTime);

        damageActive = false;
        spriteRenderer.color = hiddenColor;

        yield return new WaitForSeconds(recoveryTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player1"))
            return;

        P_Health player = other.GetComponent<P_Health>();
        if (player == null)
            player = other.GetComponentInParent<P_Health>();

        if (player != null && !player.IsDead)
            currentTarget = player;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player1"))
            return;

        P_Health player = other.GetComponent<P_Health>();
        if (player == null)
            player = other.GetComponentInParent<P_Health>();

        if (player != null && player == currentTarget)
            currentTarget = null;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player1"))
            return;

        P_Health player = other.GetComponent<P_Health>();
        if (player == null)
            player = other.GetComponentInParent<P_Health>();

        if (player != null && !player.IsDead)
            currentTarget = player;

        if (!damageActive)
            return;

        if (player != null && !player.IsDead)
        {
            Vector2 dir = DirectionToVector(facing);
            player.HitEnemy(knockback, dir);
            damageActive = false;
        }
    }

    Vector2 DirectionToVector(FacingDirection dir)
    {
        switch (dir)
        {
            case FacingDirection.Right: return Vector2.right;
            case FacingDirection.Left:  return Vector2.left;
            case FacingDirection.Up:    return Vector2.up;
            case FacingDirection.Down:  return Vector2.down;
            default: return Vector2.right;
        }
    }
}