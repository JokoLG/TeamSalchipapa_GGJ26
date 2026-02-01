using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SpriteRenderer))]
public class P_Sword : MonoBehaviour
{
    public bool isActive = true;

    [Header("Timing (seconds)")]
    public float delayTime = 0.05f;
    public float activeTime = 0.22f;
    public float cooldownTime = 0.11f;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color flashColor = Color.magenta;

    [Header("Hitbox")]
    public Collider2D hitboxCollider;

    [Header("Combat")]
    [SerializeField] private float swordKnockback = 0.4f;

    [Tooltip("Current facing direction of the player")]
    [SerializeField] public FacingDirection playerFacing;

    private SpriteRenderer swordHitbox;
    private float timer = 0f;

    private bool isStartingUp = false;
    private bool isAttacking = false;
    private bool isOnCooldown = false;

    public bool IsBusy => isStartingUp || isAttacking || isOnCooldown;

    void Awake()
    {
        swordHitbox = GetComponent<SpriteRenderer>();
        swordHitbox.color = normalColor;

        hitboxCollider = GetComponent<BoxCollider2D>();
        hitboxCollider.enabled = false;
    }

    void Update()
    {
        if (isStartingUp)
        {
            timer += Time.deltaTime;
            if (timer >= delayTime) BeginActive();
        }
        else if (isAttacking)
        {
            timer += Time.deltaTime;
            if (timer >= activeTime) EndAttack();
        }
        else if (isOnCooldown)
        {
            timer += Time.deltaTime;
            if (timer >= cooldownTime) EndCooldown();
        }
    }

    public bool TryAttack()
    {
        if (IsBusy) return false;

        StartAttack();
        return true;
    }

    void StartAttack()
    {
        isStartingUp = true;
        timer = 0f;
    }

    void BeginActive()
    {
        isStartingUp = false;
        isAttacking = true;
        timer = 0f;

        swordHitbox.color = flashColor;
        hitboxCollider.enabled = true;
    }

    void EndAttack()
    {
        isAttacking = false;
        isOnCooldown = true;
        timer = 0f;

        swordHitbox.color = normalColor;
        hitboxCollider.enabled = false;
    }

    void EndCooldown()
    {
        isOnCooldown = false;
        timer = 0f;
    }

    // ---------------- HIT DETECTION ----------------

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only hit while the sword is actually active
        if (!isAttacking) return;

        EnemyMovement_Free enemy = other.GetComponent<EnemyMovement_Free>();
        if (enemy == null) return;

        enemy.HitSword(swordKnockback, playerFacing);
    }

    // ---------------- EXTERNAL SETTERS ----------------

    // Call this from your player movement when facing changes
    public void SetFacing(FacingDirection dir)
    {
        playerFacing = dir;
    }
}
