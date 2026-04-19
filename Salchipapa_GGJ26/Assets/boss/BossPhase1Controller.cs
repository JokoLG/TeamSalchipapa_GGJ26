using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossPhase1Controller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private BossSwordHB swordHB;
    [SerializeField] private Animator animator;
    [SerializeField] private BossPhase1Health health;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.55f;
    [SerializeField] private int attacksBeforeStun = 3;
    [SerializeField] private float stunDuration = 3f;

    [Header("State Names")]
    [SerializeField] private string idleState = "Idle";
    [SerializeField] private string upState = "Up";
    [SerializeField] private string downState = "Down";
    [SerializeField] private string leftState = "Left";
    [SerializeField] private string rightState = "Right";
    [SerializeField] private string hurtState = "Hurt";

    private FacingDirection facing = FacingDirection.Right;
    private bool isAttacking = false;
    private bool isStunned = false;
    private bool isDead = false;
    private float attackCooldownTimer = 0f;
    private int attacksUsed = 0;
    private string currentState = "";

    public FacingDirection Facing => facing;
    public bool IsStunned => isStunned;
    public bool IsDead => isDead;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player1");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (isDead || player == null)
            return;

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        if (isStunned)
        {
            rb.linearVelocity = Vector2.zero;
            PlayState(hurtState);
            return;
        }

        Vector2 toPlayer = player.position - transform.position;
        Vector2 cardinalDir = GetCardinalDirection(toPlayer);

        if (cardinalDir != Vector2.zero)
            facing = VectorToFacing(cardinalDir);

        UpdateWeaponPivot();

        if (swordHB != null)
            swordHB.SetFacing(facing);

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (swordHB != null && swordHB.HasTarget && attackCooldownTimer <= 0f)
        {
            StartCoroutine(AttackRoutine());
            return;
        }

        rb.linearVelocity = cardinalDir * moveSpeed;
        PlayMoveState();
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        yield return swordHB.PlayAttack();

        attacksUsed++;
        attackCooldownTimer = attackCooldown;
        isAttacking = false;

        if (attacksUsed >= attacksBeforeStun)
        {
            attacksUsed = 0;
            StartCoroutine(StunRoutine());
        }
    }

    IEnumerator StunRoutine()
    {
        isStunned = true;
        rb.linearVelocity = Vector2.zero;

        if (health != null)
            health.SetStunned(true);

        yield return new WaitForSeconds(stunDuration);

        if (!isDead)
        {
            isStunned = false;

            if (health != null)
                health.SetStunned(false);
        }
    }

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
    }

    Vector2 GetCardinalDirection(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            return delta.x > 0 ? Vector2.right : Vector2.left;
        else
            return delta.y > 0 ? Vector2.up : Vector2.down;
    }

    FacingDirection VectorToFacing(Vector2 dir)
    {
        if (dir == Vector2.right) return FacingDirection.Right;
        if (dir == Vector2.left) return FacingDirection.Left;
        if (dir == Vector2.up) return FacingDirection.Up;
        return FacingDirection.Down;
    }

    void UpdateWeaponPivot()
    {
        if (weaponPivot == null) return;

        switch (facing)
        {
            case FacingDirection.Right: weaponPivot.localRotation = Quaternion.Euler(0, 0, 0); break;
            case FacingDirection.Up:    weaponPivot.localRotation = Quaternion.Euler(0, 0, 90); break;
            case FacingDirection.Left:  weaponPivot.localRotation = Quaternion.Euler(0, 0, 180); break;
            case FacingDirection.Down:  weaponPivot.localRotation = Quaternion.Euler(0, 0, 270); break;
        }
    }

    void PlayMoveState()
    {
        switch (facing)
        {
            case FacingDirection.Up:    PlayState(upState); break;
            case FacingDirection.Down:  PlayState(downState); break;
            case FacingDirection.Left:  PlayState(leftState); break;
            case FacingDirection.Right: PlayState(rightState); break;
        }
    }

    void PlayState(string stateName)
    {
        if (animator == null || string.IsNullOrEmpty(stateName))
            return;

        if (currentState == stateName)
            return;

        animator.Play(stateName, 0, 0f);
        currentState = stateName;
    }
}