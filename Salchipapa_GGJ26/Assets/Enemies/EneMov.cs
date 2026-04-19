using UnityEngine;

public class EneMov : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform[] playerObjectives = new Transform[4];

    [Header("Detection Zone")]
    [SerializeField] private CircleCollider2D detectionZone;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float arriveDistance = 0.15f;

    private Rigidbody2D rb;
    private EneCom combat;
    private EneAtk attack;
    private EneAni ani;

    private bool isActive;
    private Transform targetObjective;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        combat = GetComponent<EneCom>();
        attack = GetComponent<EneAtk>();
        ani = GetComponent<EneAni>();
        isActive = true;
    }

    void Update()
    {
        if (!isActive)
        {
            StopMovement();
            return;
        }

        if (combat != null && !combat.CanMove)
        {
            StopMovement();
            return;
        }

        if (attack != null && attack.ShouldBlockMovement)
        {
            StopMovement();
            return;
        }

        if (detectionZone == null)
        {
            StopMovement();
            return;
        }

        Vector2 currentPos = transform.position;
        Vector2 destination = GetCurrentDestination(currentPos);
        Vector2 toDestination = destination - currentPos;

        if (toDestination.magnitude <= arriveDistance)
        {
            StopMovement();
            return;
        }

        Vector2 moveDir = ChooseCardinalDirection(toDestination);
        transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);

        if (ani != null)
            ani.SetMoving(true);

        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    public void ActivateEnemy()
    {
        isActive = true;
    }

    public void DeactivateEnemy()
    {
        isActive = false;
        StopMovement();
    }

    Vector2 GetCurrentDestination(Vector2 currentPos)
    {
        if (HasAnyObjectiveInDetectionZone())
        {
            targetObjective = GetClosestObjectiveInDetectionZone(currentPos);

            if (targetObjective != null)
                return targetObjective.position;
        }

        targetObjective = null;
        return GetDetectionZoneCenterWorld();
    }

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

    Transform GetClosestObjectiveInDetectionZone(Vector2 currentPos)
    {
        if (detectionZone == null) return null;

        Transform best = null;
        float bestDist = float.PositiveInfinity;

        for (int i = 0; i < playerObjectives.Length; i++)
        {
            Transform t = playerObjectives[i];
            if (t == null) continue;
            if (!detectionZone.OverlapPoint(t.position)) continue;

            float dist = Vector2.SqrMagnitude((Vector2)t.position - currentPos);

            if (dist < bestDist)
            {
                bestDist = dist;
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

    void StopMovement()
    {
        if (ani != null)
            ani.SetMoving(false);

        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}