using UnityEngine;

public enum FacingDirection { Up, Down, Left, Right }

[RequireComponent(typeof(Rigidbody2D))]
public class P_Movement : MonoBehaviour
{
    public Transform weaponPivot;

    [Header("Weapon References")]
    public P_Sword sword;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public bool isMoving = false;

    public FacingDirection facing = FacingDirection.Right;

    private Rigidbody2D rb;
    private Vector2 movement;

    private FacingDirection lastPressed = FacingDirection.Right;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Track last pressed movement key
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) lastPressed = FacingDirection.Right;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))  lastPressed = FacingDirection.Left;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))    lastPressed = FacingDirection.Up;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))  lastPressed = FacingDirection.Down;

        // Decide actual move direction (4-way only)
        FacingDirection? moveDir = GetMoveDirection(lastPressed);

        if (moveDir.HasValue)
        {
            movement = DirToVector(moveDir.Value);
            isMoving = true;

            // Facing now matches actual motion direction
            facing = moveDir.Value;
        }
        else
        {
            movement = Vector2.zero;
            isMoving = false;
        }

        UpdateWeaponPivot();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            sword?.TryAttack();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    FacingDirection? GetMoveDirection(FacingDirection preferred)
    {
        // Preferred direction wins if still held
        if (IsHeld(preferred))
            return preferred;

        // Otherwise fall back to any held direction.
        // Order here is only used when preferred is released.
        if (IsHeld(FacingDirection.Right)) return FacingDirection.Right;
        if (IsHeld(FacingDirection.Left))  return FacingDirection.Left;
        if (IsHeld(FacingDirection.Up))    return FacingDirection.Up;
        if (IsHeld(FacingDirection.Down))  return FacingDirection.Down;

        return null;
    }

    bool IsHeld(FacingDirection dir)
    {
        switch (dir)
        {
            case FacingDirection.Right: return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
            case FacingDirection.Left:  return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            case FacingDirection.Up:    return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            case FacingDirection.Down:  return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            default: return false;
        }
    }

    Vector2 DirToVector(FacingDirection dir)
    {
        switch (dir)
        {
            case FacingDirection.Right: return Vector2.right;
            case FacingDirection.Left:  return Vector2.left;
            case FacingDirection.Up:    return Vector2.up;
            case FacingDirection.Down:  return Vector2.down;
            default: return Vector2.zero;
        }
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
}
