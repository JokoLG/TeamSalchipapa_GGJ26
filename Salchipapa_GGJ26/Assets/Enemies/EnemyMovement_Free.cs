using System.Collections;
using UnityEngine;

public class EnemyMovement_Free : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform[] playerObjectives = new Transform[4];

    [Header("Player (for left/right facing when stopped)")]
    [SerializeField] private Transform player;

    [Header("Detection Zone")]
    [Tooltip("External circle collider used as the aggro/home zone.")]
    [SerializeField] private CircleCollider2D detectionZone;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float arriveDistance = 0.15f;

    [Header("Facing")]
    [Tooltip("If true, also face the movement direction while moving (cardinal).")]
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

        if (detectionZone == null)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 currentPos = transform.position;

        bool anyObjectiveInZone = HasAnyObjectiveInDetectionZone();
        Vector2 destination;
        bool goingToObjective = false;

        if (anyObjectiveInZone)
        {
            targetObjective = GetClosestObjectiveInDetectionZone();

            if (targetObjective == null)
            {
                destination = GetDetectionZoneCenterWorld();
            }
            else
            {
                destination = targetObjective.position;
                goingToObjective = true;
            }
        }
        else
        {
            targetObjective = null;
            destination = GetDetectionZoneCenterWorld();
        }

        Vector2 toDestination = destination - currentPos;

        // ARRIVED
        if (toDestination.magnitude <= arriveDistance)
        {
            if (!stopped)
            {
                stopped = true;
                FacePlayerLeftRightIfAvailable();

                if (goingToObjective)
                    Attack();
            }
            else
            {
                FacePlayerLeftRightIfAvailable();
            }

            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        // MOVING
        stopped = false;

        Vector2 moveDir = ChooseCardinalDirection(toDestination);

        if (faceMoveDirectionWhileMoving)
            FaceCardinal(moveDir);

        float currentSpeed = baseMoveSpeed * speedMultiplier;
        transform.position += (Vector3)(moveDir * currentSpeed * Time.deltaTime);

        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    // -------------------- Public hit API --------------------

    public void HitFireball(float knockback, Vector2 direction)
    {
        TryApplyHit(knockback, direction);
    }

    public void HitSword(float knockback, FacingDirection dir)
    {
        TryApplyHit(knockback, dir);
    }

    void TryApplyHit(float knockback, FacingDirection attackDir)
    {
        if (isInvincible) return;
        ApplyKnockback(knockback, attackDir);
        StartIFrames();
    }

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

    void ApplyKnockback(float amount, FacingDirection attackDir)
    {
        if (amount <= 0f) return;

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        if (speedRecoverRoutine != null)
            StopCoroutine(speedRecoverRoutine);

        Vector2 knockDir = DirToVector(attackDir);
        knockbackRoutine = StartCoroutine(KnockbackRoutine(knockDir, amount));
    }

    void ApplyKnockback(float amount, Vector2 direction)
    {
        if (amount <= 0f) return;

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        if (speedRecoverRoutine != null)
            StopCoroutine(speedRecoverRoutine);

        Vector2 knockDir = direction;
        knockbackRoutine = StartCoroutine(KnockbackRoutine(knockDir, amount));
    }

    IEnumerator KnockbackRoutine(Vector2 dir, float amount)
    {
        isKnockedback = true;
        stopped = false;

        if (rb != null) rb.linearVelocity = Vector2.zero;

        float timer = 0f;
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

            float u = Mathf.Clamp01(t / duration);
            u = 1f - Mathf.Pow(1f - u, 3f);

            speedMultiplier = Mathf.Lerp(start, 1f, u);
            yield return null;
        }

        speedMultiplier = 1f;
        speedRecoverRoutine = null;
    }

    // -------------------- Detection / Objective logic --------------------

    bool HasAnyObjectiveInDetectionZone()
    {
        if (detectionZone == null) return false;

        for (int i = 0; i < playerObjectives.Length; i++)
        {
            Transform t = playerObjectives[i];
            if (t == null) continue;

            if (detectionZone.OverlapPoint(t.position))
                return true;
        }

        return false;
    }

    Transform GetClosestObjectiveInDetectionZone()
    {
        if (detectionZone == null) return null;

        Transform best = null;
        float bestDist = float.PositiveInfinity;
        Vector2 pos = transform.position;

        for (int i = 0; i < playerObjectives.Length; i++)
        {
            Transform t = playerObjectives[i];
            if (t == null) continue;
            if (!detectionZone.OverlapPoint(t.position)) continue;

            float d = Vector2.SqrMagnitude((Vector2)t.position - pos);
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }

        return best;
    }

    Vector2 GetDetectionZoneCenterWorld()
    {
        if (detectionZone == null)
            return transform.position;

        return detectionZone.transform.TransformPoint(detectionZone.offset);
    }

    Vector2 ChooseCardinalDirection(Vector2 to)
    {
        if (Mathf.Abs(to.x) >= Mathf.Abs(to.y))
            return (to.x >= 0f) ? Vector2.right : Vector2.left;
        else
            return (to.y >= 0f) ? Vector2.up : Vector2.down;
    }

    // -------------------- Facing helpers --------------------

    // Only face LEFT or RIGHT when stopped
    void FacePlayerLeftRightIfAvailable()
    {
        if (player == null) return;

        float xDiff = player.position.x - transform.position.x;

        if (Mathf.Abs(xDiff) <= 0.0001f)
            return;

        if (xDiff >= 0f)
            FaceHorizontal(true);
        else
            FaceHorizontal(false);
    }

    void FaceHorizontal(bool faceRight)
    {
        float zRot = faceRight ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0f, 0f, zRot);
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