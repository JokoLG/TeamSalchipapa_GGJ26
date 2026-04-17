using UnityEngine;

public class P_AnimatorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private P_Movement movementScript;
    [SerializeField] private Animator animator;

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

    private RuntimeAnimatorController currentController;
    private string currentState = "";

    private bool manualStateLock = false;
    private string lockedState = "";

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (movementScript == null)
            movementScript = GetComponentInParent<P_Movement>();
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

        if (!movementScript.isMoving)
        {
            targetState = idleState;
        }
        else
        {
            switch (movementScript.facing)
            {
                case FacingDirection.Up:
                    targetState = upState;
                    break;

                case FacingDirection.Down:
                    targetState = downState;
                    break;

                case FacingDirection.Left:
                    targetState = leftState;
                    break;

                case FacingDirection.Right:
                    targetState = rightState;
                    break;

                default:
                    targetState = idleState;
                    break;
            }
        }

        PlayState(targetState);
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
    }

    public void StopManualState()
    {
        manualStateLock = false;
        lockedState = "";
        currentState = "";
    }
}