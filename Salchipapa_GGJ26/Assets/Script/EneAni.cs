using System.Collections;
using UnityEngine;

public class EneAni : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SFX_Manager sfxManager;

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

    void Update()
    {
        RefreshAnimation();
    }

    public void SetMoving(bool moving)
    {
        if (isDead) return;
        isMoving = moving;
    }

    public void PlayAttackFor(float duration)
    {
        if (isDead) return;

        if (sfxManager != null && !string.IsNullOrEmpty(attackSound))
            sfxManager.Play(attackSound);

        StartAnimationLock(attackingState, duration);
    }

    public void PlayHurtFor(float duration)
    {
        if (isDead) return;

        if (sfxManager != null && !string.IsNullOrEmpty(hurtSound))
            sfxManager.Play(hurtSound);

        StartAnimationLock(hurtState, duration);
    }

    public void PlayStunFor(float duration)
    {
        if (isDead) return;

        if (sfxManager != null && !string.IsNullOrEmpty(stunSound))
            sfxManager.Play(stunSound);

        StartAnimationLock(hurtState, duration);
    }

    public void PlayBlock()
    {
        if (isDead) return;

        if (sfxManager != null && !string.IsNullOrEmpty(blockSound))
            sfxManager.Play(blockSound);
    }

    public void PlayDie()
    {
        if (isDead) return;

        isDead = true;
        isMoving = false;
        animationLocked = false;

        if (animationLockRoutine != null)
        {
            StopCoroutine(animationLockRoutine);
            animationLockRoutine = null;
        }

        if (sfxManager != null && !string.IsNullOrEmpty(dieSound))
            sfxManager.Play(dieSound);
    }

    void StartAnimationLock(string stateName, float duration)
    {
        if (string.IsNullOrEmpty(stateName))
            return;

        if (animationLockRoutine != null)
            StopCoroutine(animationLockRoutine);

        animationLockRoutine = StartCoroutine(AnimationLockRoutine(stateName, duration));
    }

    IEnumerator AnimationLockRoutine(string stateName, float duration)
    {
        animationLocked = true;
        PlayState(stateName);

        yield return new WaitForSeconds(Mathf.Max(0.01f, duration));

        animationLocked = false;
        animationLockRoutine = null;
    }

    void RefreshAnimation()
    {
        if (animator == null) return;

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
        if (string.IsNullOrEmpty(stateName)) return;
        if (currentState == stateName) return;

        animator.Play(stateName);
        currentState = stateName;
    }
}