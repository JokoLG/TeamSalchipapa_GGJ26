using System.Collections;
using UnityEngine;

public class EneAtk : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyAttackHitbox attackHitbox;

    [Header("Attack")]
    [SerializeField] private float attackStartup = 0.08f;
    [SerializeField] private float attackDuration = 0.35f;
    [SerializeField] private float attackCooldown = 0.90f;
    [SerializeField] private float attackKnockback = 1.75f;

    private EneCom combat;
    private EneAni ani;

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
        ani = GetComponent<EneAni>();
    }

    void Update()
    {
        if (!isActive) return;

        if (combat != null && !combat.CanAttack)
        {
            isAttacking = false;
            return;
        }

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

        if (ani != null)
            ani.PlayAttackFor(attackDuration);

        yield return new WaitForSeconds(attackStartup);

        if (combat != null && combat.CanAttack && attackHitbox != null && attackHitbox.HasTarget)
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

        float remainingAttackTime = attackDuration - attackStartup;
        if (remainingAttackTime > 0f)
            yield return new WaitForSeconds(remainingAttackTime);

        cooldownTimer = attackCooldown;
        isAttacking = false;
    }
}