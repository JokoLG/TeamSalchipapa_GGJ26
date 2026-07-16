using System.Collections;
using UnityEngine;

public class P_AnimatorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private P_Movement movementScript;
    [SerializeField] private Animator animator;
    [SerializeField] private P_SharkRush sharkRush;

    [Header("Visual Effects")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashInterval = 0.08f;
    [SerializeField] private Color flashColor = Color.red;

    [Header("Override Controllers")]
    [SerializeField] private AnimatorOverrideController baseOverride;
    [SerializeField] private AnimatorOverrideController witchOverride;
    [SerializeField] private AnimatorOverrideController sharkOverride;
    [SerializeField] private AnimatorOverrideController odyOverride;

    [Header("State Names")]
    [SerializeField] private string idleState = "Idle";
    [SerializeField] private string upState = "Up";
    [SerializeField] private string downState = "Down";
    [SerializeField] private string leftState = "Left";
    [SerializeField] private string rightState = "Right";
    [SerializeField] private string hurtState = "Hurt";

    [Header("Rush State Names")]
    [SerializeField] private string rushUpState = "RushUp";
    [SerializeField] private string rushDownState = "RushDown";
    [SerializeField] private string rushLeftState = "RushLeft";
    [SerializeField] private string rushRightState = "RushRight";

    private RuntimeAnimatorController currentController;
    private string currentState = "";

    private bool manualStateLock = false;
    private string lockedState = "";

    private Coroutine flashCoroutine;
    private Color originalColor = Color.white;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (movementScript == null)
            movementScript = GetComponentInParent<P_Movement>();

        if (sharkRush == null && movementScript != null)
            sharkRush = movementScript.sharkRush;

        if (sharkRush == null)
            sharkRush = GetComponentInParent<P_SharkRush>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (animator == null || movementScript == null)
            return;

        UpdateOverrideController();

        if (manualStateLock)
        {
            PlayState(lockedState);
            return;
        }

        UpdateAnimationState();
    }

    void UpdateOverrideController()
    {
        AnimatorOverrideController targetController = GetTargetOverrideController();

        if (targetController == null)
            return;

        if (currentController == targetController)
            return;

        animator.runtimeAnimatorController = targetController;
        currentController = targetController;

        // Force current state to refresh after swapping controller
        currentState = "";
    }

    AnimatorOverrideController GetTargetOverrideController()
    {
        switch (movementScript.weapon)
        {
            case MaskWeapon.Fireball:
                return witchOverride;

            case MaskWeapon.Shark:
                return sharkOverride;

            case MaskWeapon.Sword:
                return odyOverride;

            case MaskWeapon.None:
            default:
                return baseOverride;
        }
    }

    void UpdateAnimationState()
    {
        string targetState;

        // Special shark rush handling
        if (movementScript.weapon == MaskWeapon.Shark && sharkRush != null)
        {
            // While charging: use normal shark movement animations in facing direction
            if (sharkRush.state == SharkState.Charging)
            {
                targetState = GetNormalDirectionalState();
                PlayState(targetState);
                return;
            }

            // While actually rushing: use RushLeft / RushRight / RushUp / RushDown
            if (sharkRush.state == SharkState.Rushing)
            {
                targetState = GetRushDirectionalState();
                PlayState(targetState);
                return;
            }
        }

        // Normal animation logic
        if (!movementScript.isMoving)
        {
            targetState = idleState;
        }
        else
        {
            targetState = GetNormalDirectionalState();
        }

        PlayState(targetState);
    }

    string GetNormalDirectionalState()
    {
        switch (movementScript.facing)
        {
            case FacingDirection.Up:
                return upState;

            case FacingDirection.Down:
                return downState;

            case FacingDirection.Left:
                return leftState;

            case FacingDirection.Right:
                return rightState;

            default:
                return idleState;
        }
    }

    string GetRushDirectionalState()
    {
        switch (movementScript.facing)
        {
            case FacingDirection.Up:
                return rushUpState;

            case FacingDirection.Down:
                return rushDownState;

            case FacingDirection.Left:
                return rushLeftState;

            case FacingDirection.Right:
                return rushRightState;

            default:
                return rushRightState;
        }
    }

    void PlayState(string stateName)
    {
        if (string.IsNullOrEmpty(stateName))
            return;

        if (currentState == stateName)
            return;

        animator.Play(stateName, 0, 0f);
        currentState = stateName;
    }

    public void PlayHurt()
    {
        manualStateLock = true;
        lockedState = hurtState;
        currentState = "";
        PlayState(hurtState);

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(DamageFlash());
    }

    public void StopManualState()
    {
        manualStateLock = false;
        lockedState = "";
        currentState = "";

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    private IEnumerator DamageFlash()
    {
        if (spriteRenderer == null)
            yield break;

        while (manualStateLock)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashInterval);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.color = originalColor;
    }
}