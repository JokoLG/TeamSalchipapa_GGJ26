using System.Collections;
using UnityEngine;

public class EnemyMovement_Free : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform[] playerObjectives = new Transform[4];

    [Header("Player (for facing when stopped)")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float arriveDistance = 0.15f;

    [Header("Facing")]
    [Tooltip("If true, also face the objective while moving (cardinal).")]
    [SerializeField] private bool faceMoveDirectionWhileMoving = true;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 0.08f;

    [Header("Invincibility")]
    [SerializeField] private float iFramesDuration = 0.25f;

    [Header("Speed Recovery After Hit")]
    [Tooltip("Speed multiplier immediately after knockback ends (0.1 = 10%).")]
    [Range(0f, 1f)]
    [SerializeField] private float postHitSpeedMultiplier = 0.10f;

    [Tooltip("How long it takes to ramp back to normal speed after knockback ends.")]
    [SerializeField] private float speedRecoverTime = 0.20f;

    private Transform targetObjective;
    private bool stopped;

    private Rigidbody2D rb;

    // Knockback state
    private bool isKnockedback = false;
    private Coroutine knockbackRoutine;

    // Hit invincibility
    private bool isInvincible = false;
    private Coroutine iFramesRoutine;

    // Speed ramp
    private float baseMoveSpeed;
    private float speedMultiplier = 1f;
    private Coroutine speedRecoverRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseMoveSpeed = moveSpeed;
    }

    void Update()
    {
        // If being knocked back, don't run AI movement this frame
        if (isKnockedback)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        targetObjective = GetClosestObjective();
        if (targetObjective == null) return;

        Vector2 pos = transform.position;
        Vector2 objPos = targetObjective.position;
        Vector2 toObjective = objPos - pos;

        // ARRIVED: stop, face player, attack once
        if (toObjective.magnitude <= arriveDistance)
        {
            if (!stopped)
            {
                stopped = true;
                FacePlayerIfAvailable();
                Attack();
            }
            else
            {
                FacePlayerIfAvailable();
            }
            return;
        }

        // MOVING
        stopped = false;

        // Choose 4-dir step toward objective
        Vector2 moveDir = ChooseCardinalDirection(toObjective);

        // Face movement direction while moving (optional)
        if (faceMoveDirectionWhileMoving)
            FaceCardinal(moveDir);

        // Move (apply speed multiplier)
        float currentSpeed = baseMoveSpeed * speedMultiplier;
        transform.position += (Vector3)moveDir * (currentSpeed * Time.deltaTime);

        // Kill any residual RB velocity if you're moving by transform
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    // -------------------- Public hit API --------------------

    // Fireball now uses Vector2 direction (e.g. fireball travel direction)
    public void HitFireball(float knockback, Vector2 direction)
    {
        TryApplyHit(knockback, direction);
    }

    // Sword stays cardinal (FacingDirection)
    public void HitSword(float knockback, FacingDirection dir)
    {
        TryApplyHit(knockback, dir);
    }

    // Gate all hits through i-frames (FacingDirection version)
    void TryApplyHit(float knockback, FacingDirection attackDir)
    {
        if (isInvincible) return;
        ApplyKnockback(knockback, attackDir);
        StartIFrames();
    }

    // Gate all hits through i-frames (Vector2 version)
    void TryApplyHit(float knockback, Vector2 direction)
    {
        if (isInvincible) return;

        if (direction.sqrMagnitude < 0.0001f) return;

        ApplyKnockback(knockback, direction.normalized);
        StartIFrames();
    }

    // -------------------- Invincibility --------------------

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

    // -------------------- Knockback --------------------

    // Knockback from a cardinal direction (sword)
    void ApplyKnockback(float amount, FacingDirection attackDir)
    {
        if (amount <= 0f) return;

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        if (speedRecoverRoutine != null)
            StopCoroutine(speedRecoverRoutine);

        // Convert facing dir -> vector and push AWAY from it
        Vector2 knockDir = DirToVector(attackDir);

        knockbackRoutine = StartCoroutine(KnockbackRoutine(knockDir, amount));
    }

    // Knockback from a continuous direction (fireball)
    void ApplyKnockback(float amount, Vector2 direction)
    {
        if (amount <= 0f) return;

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        if (speedRecoverRoutine != null)
            StopCoroutine(speedRecoverRoutine);

        // Push AWAY from incoming direction
        Vector2 knockDir = direction;

        knockbackRoutine = StartCoroutine(KnockbackRoutine(knockDir, amount));
    }

    IEnumerator KnockbackRoutine(Vector2 dir, float amount)
    {
        isKnockedback = true;
        stopped = false; // getting hit interrupts idle/attack stance

        if (rb != null) rb.linearVelocity = Vector2.zero;

        float timer = 0f;

        // Interpret "amount" as total displacement over the duration
        float kbSpeed = amount / Mathf.Max(0.0001f, knockbackDuration);

        while (timer < knockbackDuration)
        {
            transform.position += (Vector3)(dir * kbSpeed * Time.deltaTime);

            if (rb != null) rb.linearVelocity = Vector2.zero;

            timer += Time.deltaTime;
            yield return null;
        }

        isKnockedback = false;
        knockbackRoutine = null;

        // After knockback ends: slow to 10% then ramp back up quickly
        StartSpeedRecover();
    }

    // -------------------- Speed recovery --------------------

    void StartSpeedRecover()
    {
        speedMultiplier = Mathf.Clamp01(postHitSpeedMultiplier);

        if (speedRecoverRoutine != null)
            StopCoroutine(speedRecoverRoutine);

        speedRecoverRoutine = StartCoroutine(SpeedRecoverRoutine());
    }

    IEnumerator SpeedRecoverRoutine()
    {
        float t = 0f;
        float start = speedMultiplier;
        float duration = Mathf.Max(0.0001f, speedRecoverTime);

        while (t < duration)
        {
            t += Time.deltaTime;

            // quick ramp (ease-out)
            float u = Mathf.Clamp01(t / duration);
            u = 1f - Mathf.Pow(1f - u, 3f);

            speedMultiplier = Mathf.Lerp(start, 1f, u);
            yield return null;
        }

        speedMultiplier = 1f;
        speedRecoverRoutine = null;
    }

    // -------------------- Objective logic --------------------

    Transform GetClosestObjective()
    {
        Transform best = null;
        float bestDist = float.PositiveInfinity;
        Vector2 pos = transform.position;

        for (int i = 0; i < playerObjectives.Length; i++)
        {
            Transform t = playerObjectives[i];
            if (t == null) continue;

            float d = Vector2.SqrMagnitude((Vector2)t.position - pos);
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }
        return best;
    }

    Vector2 ChooseCardinalDirection(Vector2 to)
    {
        if (Mathf.Abs(to.x) >= Mathf.Abs(to.y))
            return (to.x >= 0) ? Vector2.right : Vector2.left;
        else
            return (to.y >= 0) ? Vector2.up : Vector2.down;
    }

    // -------------------- Facing helpers --------------------

    void FacePlayerIfAvailable()
    {
        if (player == null) return;

        Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;
        if (toPlayer.sqrMagnitude > 0.0001f)
            FaceToward(toPlayer);
    }

    void FaceCardinal(Vector2 dir)
    {
        float zRot =
            (dir == Vector2.right) ? 0f :
            (dir == Vector2.up) ? 90f :
            (dir == Vector2.left) ? 180f :
            270f;

        transform.rotation = Quaternion.Euler(0f, 0f, zRot);
    }

    void FaceToward(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
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

    void Attack()
    {
        // empty for now
    }
}
