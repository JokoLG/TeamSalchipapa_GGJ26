using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class P_Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int hitsToDie = 3;

    [Header("Invincibility")]
    [SerializeField] private float iFramesDuration = 0.35f;

    [Header("Knockback / Stun")]
    [SerializeField] private float knockbackDuration = 0.10f;

    [Header("References")]
    [SerializeField] private P_AnimatorController animatorController;
    [SerializeField] private P_SoundHandler soundPlayer;

    private Rigidbody2D rb;
    private P_Movement movement;

    private int hitsTaken = 0;

    private bool isInvincible = false;
    private bool isDead = false;
    private bool isKnockedback = false;

    private Coroutine iFramesRoutine;
    private Coroutine knockbackRoutine;

    public bool IsDead => isDead;
    public bool IsInvincible => isInvincible;
    public int HitsTaken => hitsTaken;
    public int HitsRemaining => Mathf.Max(0, hitsToDie - hitsTaken);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<P_Movement>();

        if (animatorController == null)
            animatorController = GetComponentInChildren<P_AnimatorController>();

        if (soundPlayer == null)
            soundPlayer = GetComponent<P_SoundHandler>();
    }

    public void HitEnemy(float knockback, Vector2 direction)
    {
        if (isDead || isInvincible) return;

        hitsTaken++;

        PlayHurtFeedback();

        StartIFrames();
        ApplyKnockback(knockback, direction);

        if (hitsTaken >= hitsToDie)
            Die();
    }

    void PlayHurtFeedback()
    {
        if (animatorController != null)
            animatorController.PlayHurt();

        if (soundPlayer != null)
        {
            switch (hitsTaken)
            {
                case 1:
                    soundPlayer.Play("Hit_1");
                    break;

                case 2:
                    soundPlayer.Play("Hit_2");
                    break;

                default:
                    soundPlayer.Play("Death");
                    break;
            }
        }
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

        if (movement != null)
            movement.SetControlsLocked(true);

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

        if (!isDead && animatorController != null)
            animatorController.StopManualState();

        if (!isDead && movement != null)
            movement.SetControlsLocked(false);
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        if (movement != null)
            movement.SetControlsLocked(true);

        SceneManager.LoadScene("GameOver");
        gameObject.SetActive(false);
    }
}