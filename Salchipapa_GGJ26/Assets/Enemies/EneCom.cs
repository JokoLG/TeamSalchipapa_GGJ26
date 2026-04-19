using System.Collections;
using UnityEngine;

public class EneCom : MonoBehaviour
{
    [Header("Enemy Type")]
    [SerializeField] private bool isStunnable = false;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int fireballDamage = 1;
    [SerializeField] private int swordDamage = 3;

    [Header("Stun")]
    [SerializeField] private float stunDuration = 1.25f;

    [Header("Hurt Animation")]
    [SerializeField] private float hurtAnimationDuration = 0.20f;

    [Header("Attack Animation")]
    [SerializeField] private float attackDuration = 0.35f;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 0.08f;

    [Header("Invincibility")]
    [SerializeField] private float iFramesDuration = 0.25f;

    [Header("Death Drop")]
    [SerializeField] private GameObject sharkMaskPrefab;

    private Rigidbody2D rb;
    private EneAni ani;

    private int currentHealth;

    private bool isKnockedback = false;
    private bool isInvincible = false;
    private bool isDead = false;
    private bool isStunned = false;
    private bool isAttacking = false;

    private Coroutine knockbackRoutine;
    private Coroutine iFramesRoutine;
    private Coroutine stunRoutine;
    private Coroutine attackRoutine;

    public bool IsStunnable => isStunnable;
    public bool IsStunned => isStunned;
    public bool IsDead => isDead;
    public bool IsAttacking => isAttacking;

    public bool CanMove => !isDead && !isKnockedback && !isStunned && !isAttacking;
    public bool CanAttack => !isDead && !isStunned && !isAttacking;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<EneAni>();
        currentHealth = Mathf.Max(1, maxHealth);
    }

    public void StartAttack()
    {
        if (!CanAttack) return;

        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        attackRoutine = StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (ani != null)
            ani.PlayAttackFor(attackDuration);

        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
        attackRoutine = null;
    }

    public void HitSword(float knockback, FacingDirection dir)
    {
        if (isDead) return;

        if (isStunnable)
        {
            if (!isStunned)
            {
                if (ani != null) ani.PlayBlock();
                return;
            }

            HitStunnedSword(knockback, dir);
            return;
        }

        TryDamage(swordDamage, knockback, DirToVector(dir), true);
    }

    public void HitFireball(float knockback, Vector2 direction)
    {
        if (isDead) return;

        if (isStunnable)
        {
            if (!isStunned)
            {
                if (ani != null) ani.PlayBlock();
                return;
            }

            HitStunnedFireball(knockback, direction);
            return;
        }

        TryDamage(fireballDamage, knockback, direction, true);
    }

    public void HitStunnedSword(float knockback, FacingDirection dir)
    {
        if (isDead) return;
        if (!isStunnable) return;
        if (!isStunned) return;

        TryDamage(swordDamage, knockback, DirToVector(dir), true);
    }

    public void HitStunnedFireball(float knockback, Vector2 direction)
    {
        if (isDead) return;
        if (!isStunnable) return;
        if (!isStunned) return;

        TryDamage(fireballDamage, knockback, direction, true);
    }

    public void Stun()
    {
        if (isDead) return;
        if (!isStunnable) return;

        if (stunRoutine != null)
            StopCoroutine(stunRoutine);

        stunRoutine = StartCoroutine(StunRoutine());
    }

    IEnumerator StunRoutine()
    {
        isStunned = true;
        isAttacking = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (ani != null)
            ani.PlayStunFor(stunDuration);

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        stunRoutine = null;
    }

    void TryDamage(int damage, float knockback, Vector2 direction, bool playHurtFeedback)
    {
        if (isInvincible) return;
        if (damage <= 0) return;

        isAttacking = false;

        ApplyKnockback(knockback, direction);
        StartIFrames();

        if (playHurtFeedback && ani != null)
            ani.PlayHurtFor(hurtAnimationDuration);

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

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

    void ApplyKnockback(float amount, Vector2 direction)
    {
        if (amount <= 0f) return;

        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.right;
        else
            direction.Normalize();

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

    void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;

        if (ani != null)
            ani.PlayDie();

        if (sharkMaskPrefab != null)
            Instantiate(sharkMaskPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    Vector2 DirToVector(FacingDirection dir)
    {
        switch (dir)
        {
            case FacingDirection.Right: return Vector2.right;
            case FacingDirection.Left:  return Vector2.left;
            case FacingDirection.Up:    return Vector2.up;
            case FacingDirection.Down:  return Vector2.down;
            default: return Vector2.zero;
        }
    }
}