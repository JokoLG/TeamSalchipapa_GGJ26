using System.Collections;
using UnityEngine;

public class EneAtk : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyAttackHitbox attackHitbox;

    [Header("Attack")]
    [SerializeField] private float attackStartup = 0.08f;
    [SerializeField] private float attackCooldown = 0.60f;
    [SerializeField] private float attackKnockback = 0.80f;

    private EneCom combat;

    private bool isAttacking = false;
    private float cooldownTimer = 0f;
    private bool isActive = true;

    public bool IsAttacking => isAttacking;

    public bool ShouldBlockMovement
    {
        get
        {
            return isAttacking || (attackHitbox != null && attackHitbox.HasTarget);
        }
    }

    void Awake()
    {
        combat = GetComponent<EneCom>();
    }

    void Update()
    {
        if (!isActive) return;
        if (combat != null && combat.IsDead) return;

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (CanStartAttack())
            StartCoroutine(AttackRoutine());
    }

    public void ActivateAttack()
    {
        isActive = true;
    }

    public void DeactivateAttack()
    {
        isActive = false;
    }

    bool CanStartAttack()
    {
        if (isAttacking) return false;
        if (cooldownTimer > 0f) return false;
        if (attackHitbox == null) return false;
        if (!attackHitbox.HasTarget) return false;
        return true;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        yield return new WaitForSeconds(attackStartup);

        if (attackHitbox != null && attackHitbox.HasTarget)
        {
            P_Health player = attackHitbox.CurrentTarget;

            if (player != null)
            {
                Vector2 dir = player.transform.position - transform.position;

                if (dir.sqrMagnitude < 0.0001f)
                    dir = Vector2.right;
                else
                    dir.Normalize();

                player.HitEnemy(attackKnockback, dir);
            }
        }

        cooldownTimer = attackCooldown;
        isAttacking = false;
    }
}