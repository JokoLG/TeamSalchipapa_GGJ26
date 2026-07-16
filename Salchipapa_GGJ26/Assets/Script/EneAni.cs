using System.Collections;
using UnityEngine;

public class EneAni : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SFX_Manager sfxManager;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Flash Effect")]
    [SerializeField] private float flashInterval = 0.08f;
    [SerializeField] private Color hurtFlashColor = Color.red;
    [SerializeField] private Color stunFlashColor = Color.yellow;

    [Header("Animation State Names")]
    [SerializeField] private string hurtState = "Hurt";
    [SerializeField] private string idleState = "Idle";
    [SerializeField] private string movingState = "Moving";
    [SerializeField] private string attackingState = "Attacking";

    [Header("Sound IDs")]
    [SerializeField] private string hurtSound = "SmallHit";
    [SerializeField] private string stunSound = "PH5";
    [SerializeField] private string attackSound = "SmallAttack";
    [SerializeField] private string dieSound = "SmallDeath";
    [SerializeField] private string blockSound = "PH4";

    private string currentState = "";

    private bool isMoving = false;
    private bool isDead = false;

    private bool animationLocked = false;
    private Coroutine animationLockRoutine;
    private Coroutine flashRoutine;

    private Color originalColor = Color.white;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        RefreshAnimation();
    }

    public void SetMoving(bool moving)
    {
        if (isDead)
            return;

        isMoving = moving;
    }

    public void PlayAttackFor(float duration)
    {
        if (isDead)
            return;

        if (sfxManager != null && !string.IsNullOrEmpty(attackSound))
            sfxManager.Play(attackSound);

        StartAnimationLock(attackingState, duration);
    }

    public void PlayHurtFor(float duration)
    {
        if (isDead)
            return;

        if (sfxManager != null && !string.IsNullOrEmpty(hurtSound))
            sfxManager.Play(hurtSound);

        StartAnimationLock(hurtState, duration);
        StartFlash(hurtFlashColor, duration);
    }

    public void PlayStunFor(float duration)
    {
        if (isDead)
            return;

        if (sfxManager != null && !string.IsNullOrEmpty(stunSound))
            sfxManager.Play(stunSound);

        StartAnimationLock(hurtState, duration);
        StartFlash(stunFlashColor, duration);
    }

    public void PlayBlock()
    {
        if (isDead)
            return;

        if (sfxManager != null && !string.IsNullOrEmpty(blockSound))
            sfxManager.Play(blockSound);
    }

    public void PlayDie()
    {
        if (isDead)
            return;

        isDead = true;
        isMoving = false;
        animationLocked = false;

        if (animationLockRoutine != null)
        {
            StopCoroutine(animationLockRoutine);
            animationLockRoutine = null;
        }

        StopFlash();

        if (sfxManager != null && !string.IsNullOrEmpty(dieSound))
            sfxManager.Play(dieSound);
    }

    void StartAnimationLock(string stateName, float duration)
    {
        if (string.IsNullOrEmpty(stateName))
            return;

        if (animationLockRoutine != null)
            StopCoroutine(animationLockRoutine);

        animationLockRoutine =
            StartCoroutine(AnimationLockRoutine(stateName, duration));
    }

    IEnumerator AnimationLockRoutine(string stateName, float duration)
    {
        animationLocked = true;
        PlayState(stateName);

        yield return new WaitForSeconds(Mathf.Max(0.01f, duration));

        animationLocked = false;
        animationLockRoutine = null;
    }

    void StartFlash(Color color, float duration)
    {
        if (spriteRenderer == null)
            return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        spriteRenderer.color = originalColor;
        flashRoutine = StartCoroutine(FlashRoutine(color, duration));
    }

    IEnumerator FlashRoutine(Color flashColor, float duration)
    {
        float timer = 0f;
        float interval = Mathf.Max(0.01f, flashInterval);
        float flashDuration = Mathf.Max(0.01f, duration);

        while (timer < flashDuration)
        {
            spriteRenderer.color = flashColor;

            yield return new WaitForSeconds(interval);
            timer += interval;

            spriteRenderer.color = originalColor;

            yield return new WaitForSeconds(interval);
            timer += interval;
        }

        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }

    void StopFlash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    void RefreshAnimation()
    {
        if (animator == null)
            return;

        if (isDead)
        {
            PlayState(idleState);
            return;
        }

        if (animationLocked)
            return;

        if (isMoving)
            PlayState(movingState);
        else
            PlayState(idleState);
    }

    void PlayState(string stateName)
    {
        if (string.IsNullOrEmpty(stateName))
            return;

        if (currentState == stateName)
            return;

        animator.Play(stateName);
        currentState = stateName;
    }
}