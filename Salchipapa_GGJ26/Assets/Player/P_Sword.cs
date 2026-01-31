using UnityEngine;

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

    [Header("Hitbox (optional)")]
    public Collider2D hitboxCollider;

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
        if (hitboxCollider != null) hitboxCollider.enabled = true;
    }

    void EndAttack()
    {
        isAttacking = false;
        isOnCooldown = true;
        timer = 0f;

        swordHitbox.color = normalColor;
        if (hitboxCollider != null) hitboxCollider.enabled = false;
    }

    void EndCooldown()
    {
        isOnCooldown = false;
        timer = 0f;
    }
}
