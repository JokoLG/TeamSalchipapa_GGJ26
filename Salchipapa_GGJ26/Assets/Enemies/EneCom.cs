using System.Collections;
using UnityEngine;

public class EneCom : MonoBehaviour
{
    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 0.08f;

    [Header("Invincibility")]
    [SerializeField] private float iFramesDuration = 0.25f;

    [Header("Combo Kill")]
    [SerializeField] private int fireballHitsRequired = 3;
    [SerializeField] private int swordHitsRequired = 1;

    [Header("Death Drop")]
    [SerializeField] private GameObject sharkMaskPrefab;

    private Rigidbody2D rb;

    private int fireballHits = 0;
    private int swordHits = 0;

    private bool isKnockedback = false;
    private bool isInvincible = false;
    private bool isDead = false;

    private Coroutine knockbackRoutine;
    private Coroutine iFramesRoutine;

    public bool CanMove => !isKnockedback && !isDead;
    public bool IsInvincible => isInvincible;
    public bool IsDead => isDead;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // ---------------- Public Hit API ----------------

    public void HitSword(float knockback, FacingDirection dir)
    {
        if (isDead) return;

        TryApplyHit(knockback, dir);
        swordHits++;
        CheckDeath();
    }

    public void HitFireball(float knockback, Vector2 direction)
    {
        if (isDead) return;

        TryApplyHit(knockback, direction);
        fireballHits++;
        CheckDeath();
    }

    // ---------------- Hit Handling ----------------

    void TryApplyHit(float knockback, FacingDirection attackDir)
    {
        if (isInvincible) return;

        ApplyKnockback(knockback, DirToVector(attackDir));
        StartIFrames();
    }

    void TryApplyHit(float knockback, Vector2 direction)
    {
        if (isInvincible) return;
        if (direction.sqrMagnitude < 0.0001f) return;

        ApplyKnockback(knockback, direction.normalized);
        StartIFrames();
    }

    // ---------------- Invincibility ----------------

    void StartIFrames()
    {
        if (iFramesDuration <= 0f) return;

        if (iFramesRoutine != null)
            StopCoroutine(iFramesRoutine);

        iFramesRoutine = StartCoroutine(IFramesRoutine());
    }

    IEnumerator IFramesRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(iFramesDuration);
        isInvincible = false;
        iFramesRoutine = null;
    }

    // ---------------- Knockback ----------------

    void ApplyKnockback(float amount, Vector2 direction)
    {
        if (amount <= 0f) return;

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine = StartCoroutine(KnockbackRoutine(direction, amount));
    }

    IEnumerator KnockbackRoutine(Vector2 direction, float amount)
    {
        isKnockedback = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        float timer = 0f;
        float knockbackSpeed = amount / Mathf.Max(0.0001f, knockbackDuration);

        while (timer < knockbackDuration)
        {
            transform.position += (Vector3)(direction * knockbackSpeed * Time.deltaTime);

            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            timer += Time.deltaTime;
            yield return null;
        }

        isKnockedback = false;
        knockbackRoutine = null;
    }

    // ---------------- Death ----------------

    void CheckDeath()
    {
        if (fireballHits >= fireballHitsRequired || swordHits >= swordHitsRequired)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        if (sharkMaskPrefab != null)
            Instantiate(sharkMaskPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // ---------------- Helpers ----------------

    Vector2 DirToVector(FacingDirection dir)
    {
        switch (dir)
        {
            case FacingDirection.Right: return Vector2.right;
            case FacingDirection.Left: return Vector2.left;
            case FacingDirection.Up: return Vector2.up;
            case FacingDirection.Down: return Vector2.down;
            default: return Vector2.zero;
        }
    }
}