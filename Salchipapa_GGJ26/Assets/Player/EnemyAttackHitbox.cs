using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private P_Health currentTarget;

    public bool HasTarget
    {
        get
        {
            return currentTarget != null && !currentTarget.IsDead;
        }
    }

    public P_Health CurrentTarget
    {
        get
        {
            if (HasTarget) return currentTarget;
            return null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        P_Health player = other.GetComponent<P_Health>();

        if (player == null)
            player = other.GetComponentInParent<P_Health>();

        if (player != null && !player.IsDead)
            currentTarget = player;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        P_Health player = other.GetComponent<P_Health>();

        if (player == null)
            player = other.GetComponentInParent<P_Health>();

        if (player != null && player == currentTarget)
            currentTarget = null;
    }
}